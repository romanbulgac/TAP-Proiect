using DataAccess.Models;
using BlazorClient.Models;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;

namespace BlazorClient.Services;

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public UserDto? CurrentUser { get; private set; }
    public string? JwtToken { get; private set; }

    public AuthStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string? storedToken = null;
        try
        {
            storedToken = await _localStorage.GetItemAsStringAsync("authToken");
        }
        catch (InvalidOperationException)
        {
            // This can happen if JS interop is not available (e.g., during pre-rendering)
            // Or if local storage is not available.
            // In this case, treat as anonymous.
            return new AuthenticationState(_anonymous);
        }

        if (string.IsNullOrEmpty(storedToken))
        {
            return new AuthenticationState(_anonymous);
        }

        var claims = ParseClaimsFromJwt(storedToken);
        if (!claims.Any())
        {
            await ClearAuthDataAsync(); // Token is invalid or expired
            return new AuthenticationState(_anonymous);
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        JwtToken = storedToken;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", storedToken);
        CurrentUser = CreateUserFromClaims(claims);

        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticatedAsync(string token)
    {
        var claims = ParseClaimsFromJwt(token);
        if (!claims.Any())
        {
            await ClearAuthDataAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            return;
        }

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        try
        {
            await _localStorage.SetItemAsStringAsync("authToken", token);
        }
        catch (InvalidOperationException)
        {
            // Handle cases where local storage might not be available.
            // Depending on the app, this might be a critical failure or handled gracefully.
        }
        JwtToken = token;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        CurrentUser = CreateUserFromClaims(claims);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public async Task MarkUserAsLoggedOutAsync()
    {
        await ClearAuthDataAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    // New methods to match AuthService calls
    public async Task SetUser(UserDto user, string token)
    {
        CurrentUser = user;
        await MarkUserAsAuthenticatedAsync(token);
    }

    public async Task SetAnonymous()
    {
        await MarkUserAsLoggedOutAsync();
    }

    private async Task ClearAuthDataAsync()
    {
        try
        {
            await _localStorage.RemoveItemAsync("authToken");
        }
        catch (InvalidOperationException)
        {
            // Local storage might not be available.
        }
        JwtToken = null;
        _httpClient.DefaultRequestHeaders.Authorization = null;
        CurrentUser = null;
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Claims;
        }
        catch
        {
            return Enumerable.Empty<Claim>();
        }
    }

    private static UserDto? CreateUserFromClaims(IEnumerable<Claim> claims)
    {
        var userIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return new UserDto
        {
            Id = userId,
            Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty,
            Name = claims.FirstOrDefault(c => c.Type == "firstName")?.Value ?? string.Empty,
            Surname = claims.FirstOrDefault(c => c.Type == "lastName")?.Value ?? string.Empty,
            Role = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty
        };
    }
}
