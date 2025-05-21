using BlazorClient.Models;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.Json;
using BusinessLayer.DTOs;
using System.Text.Json.Serialization;

namespace BlazorClient.Services
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest loginRequest);
        Task LogoutAsync();
        Task<bool> RegisterAsync(RegisterRequest registerRequest);
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _authClient;
        private readonly NavigationManager _navigationManager;
        private readonly CustomAuthenticationStateProvider _authStateProvider;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IHttpClientFactory httpClientFactory, 
            NavigationManager navigationManager, 
            AuthenticationStateProvider authStateProvider,
            ITokenService tokenService,
            ILogger<AuthService> logger)
        {
            _authClient = httpClientFactory.CreateClient("AuthClient");
            _navigationManager = navigationManager;
            _authStateProvider = (CustomAuthenticationStateProvider)authStateProvider;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<LoginResult> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {Email}", loginRequest.Email);
                
                // Create the request content manually to avoid potential issues with HttpClient extension methods
                var content = new StringContent(
                    JsonSerializer.Serialize(loginRequest), 
                    System.Text.Encoding.UTF8, 
                    "application/json");
                
                // Post without using extension methods
                var response = await _authClient.PostAsync("api/Auth/login", content);
                
                // Always read content as string first
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Server response: {Response}", responseContent);

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Parse using System.Text.Json directly with JsonDocument for more control
                        using (JsonDocument doc = JsonDocument.Parse(responseContent))
                        {
                            JsonElement root = doc.RootElement;
                            
                            // Check if token exists in response
                            if (root.TryGetProperty("token", out JsonElement tokenElement))
                            {
                                string token = tokenElement.GetString() ?? string.Empty;
                                
                                if (!string.IsNullOrEmpty(token))
                                {
                                    _logger.LogInformation("Login successful for user: {Email}", loginRequest.Email);
                                    
                                    // Extract user object if present
                                    UserDto? user = null;
                                    if (root.TryGetProperty("user", out JsonElement userElement))
                                    {
                                        try
                                        {
                                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                                            user = JsonSerializer.Deserialize<UserDto>(userElement.GetRawText(), options);
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.LogError(ex, "Failed to deserialize user object");
                                        }
                                    }
                                    
                                    // Create login result
                                    var result = new LoginResult
                                    {
                                        Successful = true,
                                        Token = token,
                                        RefreshToken = string.Empty,
                                        ExpiresIn = 3600,
                                        User = user
                                    };
                                    
                                    // Store token and update auth state
                                    await _authStateProvider.MarkUserAsAuthenticated(
                                        result.Token,
                                        result.RefreshToken ?? string.Empty,
                                        result.ExpiresIn ?? 3600
                                    );
                                    
                                    return result;
                                }
                            }
                        }
                        
                        // If we get here, no token was found but response was successful
                        _logger.LogWarning("Login response was successful but no token found for user: {Email}", loginRequest.Email);
                        return new LoginResult { Successful = false, Error = "Server response did not contain a valid token" };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing successful login response for user {Email}", loginRequest.Email);
                        return new LoginResult { Successful = false, Error = $"Error processing server response: {ex.Message}" };
                    }
                }
                else
                {
                    _logger.LogWarning("Login failed for user {Email}: {StatusCode} {Reason} {Content}", 
                        loginRequest.Email,
                        (int)response.StatusCode,
                        response.ReasonPhrase,
                        responseContent);
                    
                    // Try to extract error message from response
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseContent))
                        {
                            if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement))
                            {
                                string message = messageElement.GetString() ?? string.Empty;
                                return new LoginResult { Successful = false, Error = message };
                            }
                        }
                    }
                    catch
                    {
                        // Ignore parsing errors and use default message
                    }
                    
                    return new LoginResult { Successful = false, Error = $"Login failed (Status {(int)response.StatusCode}): {response.ReasonPhrase}" };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during login for user {Email}", loginRequest.Email);
                return new LoginResult { Successful = false, Error = $"Exception during login: {ex.Message}" };
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                _logger.LogInformation("Logging out user");
                
                // Call server to invalidate refresh token
                var refreshToken = await _tokenService.GetRefreshTokenAsync();
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    try
                    {
                        await _authClient.PostAsync("api/Auth/logout", null);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Server-side logout failed, continuing with client-side logout");
                    }
                }
                
                // Clear tokens and update auth state
                await _authStateProvider.MarkUserAsLoggedOut();
                
                // Redirect to login page
                _navigationManager.NavigateTo("/login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                // Still try to redirect to login page
                _navigationManager.NavigateTo("/login");
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest registerRequest)
        {
            try
            {
                _logger.LogInformation("Attempting to register user: {Email}", registerRequest.Email);
                
                // Create the request content manually to avoid potential issues with HttpClient extension methods
                var content = new StringContent(
                    JsonSerializer.Serialize(registerRequest), 
                    System.Text.Encoding.UTF8, 
                    "application/json");
                
                // Post without using extension methods
                var response = await _authClient.PostAsync("api/Auth/register", content);
                
                // Always read content as string first
                var responseContent = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Registration successful for user: {Email}", registerRequest.Email);
                    
                    // For auto-login after registration (optional):
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseContent))
                        {
                            JsonElement root = doc.RootElement;
                            
                            // Check if token exists in response
                            if (root.TryGetProperty("token", out JsonElement tokenElement))
                            {
                                string token = tokenElement.GetString() ?? string.Empty;
                                
                                if (!string.IsNullOrEmpty(token))
                                {
                                    _logger.LogInformation("Auto-login after registration for user: {Email}", registerRequest.Email);
                                    
                                    // Store token and update auth state
                                    await _authStateProvider.MarkUserAsAuthenticated(
                                        token,
                                        string.Empty, // We don't handle refresh token here
                                        3600 // Default expiration
                                    );
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process auto-login after registration");
                        // Continue with successful registration even if auto-login fails
                    }
                    
                    return true;
                }
                else
                {
                    _logger.LogWarning("Registration failed for user {Email}: {StatusCode} {Reason} {Content}", 
                        registerRequest.Email,
                        (int)response.StatusCode,
                        response.ReasonPhrase,
                        responseContent);
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during registration for user {Email}", registerRequest.Email);
                return false;
            }
        }
    }
}