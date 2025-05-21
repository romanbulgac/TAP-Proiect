using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.JSInterop;
using System.Net.Http.Json;
using BlazorClient.Models;
using System.Linq;

namespace BlazorClient.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ITokenService _tokenService;
        private readonly HttpClient _httpClient;
        private readonly ILogger<CustomAuthenticationStateProvider> _logger;
        private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(
            ITokenService tokenService,
            HttpClient httpClient,
            ILogger<CustomAuthenticationStateProvider> logger)
        {
            _tokenService = tokenService;
            _httpClient = httpClient;
            _logger = logger;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var accessToken = await _tokenService.GetAccessTokenAsync();

                if (string.IsNullOrEmpty(accessToken))
                {
                    _logger.LogInformation("No authentication token found, returning anonymous state");
                    return new AuthenticationState(_anonymous);
                }

                // Check if token needs refreshing
                var isExpiringSoon = await _tokenService.IsTokenExpiringSoonAsync();
                if (isExpiringSoon)
                {
                    _logger.LogInformation("Token is expiring soon, attempting refresh");
                    await RefreshTokenAsync();
                    accessToken = await _tokenService.GetAccessTokenAsync();
                    
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        _logger.LogWarning("Token refresh failed, returning anonymous state");
                        return new AuthenticationState(_anonymous);
                    }
                }

                // Create identity with role claims for authorization
                var identity = new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt");
                var user = new ClaimsPrincipal(identity);
                
                // Enhanced logging for debugging authentication issues
                _logger.LogInformation($"Authentication state created with {identity.Claims.Count()} claims");
                string GetClaimValue(ClaimsPrincipal principal, string claimType) =>
                    principal.Claims.FirstOrDefault(c => c.Type == claimType)?.Value ?? "null";
                    
                _logger.LogInformation($"User ID: {GetClaimValue(user, ClaimTypes.NameIdentifier)}");
                _logger.LogInformation($"User Name: {user.Identity?.Name ?? "null"}");
                
                var roles = identity.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();
                
                _logger.LogInformation($"Role claims found: {string.Join(", ", roles)}");
                
                // Log IsInRole checks for common roles
                _logger.LogInformation($"IsInRole('Administrator'): {user.IsInRole("Administrator")}");
                _logger.LogInformation($"IsInRole('Admin'): {user.IsInRole("Admin")}");
                _logger.LogInformation($"IsInRole('Teacher'): {user.IsInRole("Teacher")}");
                _logger.LogInformation($"IsInRole('Student'): {user.IsInRole("Student")}");
                
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAuthenticationStateAsync");
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task MarkUserAsAuthenticated(string accessToken, string refreshToken, int expiresIn)
        {
            var success = await _tokenService.StoreTokensAsync(accessToken, refreshToken, expiresIn);
            
            if (success)
            {
                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(accessToken), "jwt"));
                var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
                NotifyAuthenticationStateChanged(authState);
            }
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _tokenService.ClearTokensAsync();
            var authState = Task.FromResult(new AuthenticationState(_anonymous));
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task RefreshTokenAsync()
        {
            try
            {
                var refreshToken = await _tokenService.GetRefreshTokenAsync();
                
                if (string.IsNullOrEmpty(refreshToken))
                {
                    _logger.LogWarning("No refresh token available");
                    return;
                }
                
                _logger.LogInformation("Attempting to refresh token");
                var response = await _httpClient.PostAsJsonAsync("api/Auth/refresh-token", new { RefreshToken = refreshToken });
                
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                    
                    if (result != null && result.Successful && !string.IsNullOrEmpty(result.Token))
                    {
                        await _tokenService.StoreTokensAsync(
                            result.Token, 
                            result.RefreshToken ?? string.Empty, 
                            result.ExpiresIn ?? 3600);
                        
                        _logger.LogInformation("Token refreshed successfully");
                    }
                    else
                    {
                        _logger.LogWarning("Refresh token response was unsuccessful or invalid");
                    }
                }
                else
                {
                    _logger.LogWarning($"Failed to refresh token: {response.StatusCode}");
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // If unauthorized, clear tokens to force re-login
                        await _tokenService.ClearTokensAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token");
            }
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            
            // Create a list to hold all claims
            var claims = new List<Claim>();
            
            // Keep track of roles to avoid duplicates
            var roleClaims = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            
            // Add all claims from token
            foreach (var claim in token.Claims)
            {
                claims.Add(claim);
                
                // Handle various formats of role claims
                if (claim.Type == "role" || 
                    claim.Type == ClaimTypes.Role || 
                    claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                {
                    if (!roleClaims.Contains(claim.Value))
                    {
                        // Use ClaimTypes.Role for standard ASP.NET Core role checks
                        claims.Add(new Claim(ClaimTypes.Role, claim.Value));
                        roleClaims.Add(claim.Value);
                    }
                }
                
                // Special handling for UserType claims
                if (claim.Type == "UserType" && !string.IsNullOrEmpty(claim.Value))
                {
                    if (!roleClaims.Contains(claim.Value))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, claim.Value));
                        roleClaims.Add(claim.Value);
                    }
                }
                
                // Always ensure Administrator and Admin roles are paired
                if (roleClaims.Contains("Administrator") && !roleClaims.Contains("Admin"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                    roleClaims.Add("Admin");
                }
                else if (roleClaims.Contains("Admin") && !roleClaims.Contains("Administrator"))
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Administrator"));
                    roleClaims.Add("Administrator");
                }
            }
            
            // Log all role claims for debugging
            var roles = claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            _logger.LogInformation($"Roles detected in JWT: {string.Join(", ", roles)}");
            
            return claims;
        }
    }
}