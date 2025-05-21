using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorClient.Services
{
    /// <summary>
    /// Service responsible for handling token storage and retrieval operations
    /// </summary>
    public interface ITokenService
    {
        Task<string> GetAccessTokenAsync();
        Task<string> GetRefreshTokenAsync();
        Task<bool> StoreTokensAsync(string accessToken, string refreshToken, int expiresInSeconds);
        Task ClearTokensAsync();
        Task<bool> IsTokenExpiringSoonAsync();
    }

    public class TokenService : ITokenService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<TokenService> _logger;
        
        // Cache the tokens in memory to reduce JS interop calls
        private string? _cachedAccessToken;
        private string? _cachedRefreshToken;
        private long? _cachedTokenExpiryTime;

        public TokenService(IJSRuntime jsRuntime, ILogger<TokenService> logger)
        {
            _jsRuntime = jsRuntime;
            _logger = logger;
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedAccessToken))
            {
                return _cachedAccessToken;
            }
            
            try
            {
                _cachedAccessToken = await _jsRuntime.InvokeAsync<string>("tokenStorage.getAccessToken");
                return _cachedAccessToken ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving access token");
                return string.Empty;
            }
        }

        public async Task<string> GetRefreshTokenAsync()
        {
            if (!string.IsNullOrEmpty(_cachedRefreshToken))
            {
                return _cachedRefreshToken;
            }
            
            try
            {
                _cachedRefreshToken = await _jsRuntime.InvokeAsync<string>("tokenStorage.getRefreshToken");
                return _cachedRefreshToken ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving refresh token");
                return string.Empty;
            }
        }

        public async Task<bool> StoreTokensAsync(string accessToken, string refreshToken, int expiresInSeconds)
        {
            try
            {
                // Update the cache
                _cachedAccessToken = accessToken;
                _cachedRefreshToken = refreshToken;
                _cachedTokenExpiryTime = DateTimeOffset.UtcNow.AddSeconds(expiresInSeconds).ToUnixTimeMilliseconds();
                
                // Store in localStorage
                return await _jsRuntime.InvokeAsync<bool>("tokenStorage.storeTokens", accessToken, refreshToken, expiresInSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error storing tokens");
                return false;
            }
        }

        public async Task ClearTokensAsync()
        {
            try
            {
                // Clear the cache
                _cachedAccessToken = null;
                _cachedRefreshToken = null;
                _cachedTokenExpiryTime = null;
                
                // Clear localStorage
                await _jsRuntime.InvokeVoidAsync("tokenStorage.clearTokens");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing tokens");
            }
        }

        public async Task<bool> IsTokenExpiringSoonAsync()
        {
            try
            {
                // Check cache first if available
                if (_cachedTokenExpiryTime.HasValue)
                {
                    var currentTimeMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    // Consider token as expiring if less than 1 minute remains
                    return (_cachedTokenExpiryTime.Value - currentTimeMs) < 60000;
                }
                
                // If no cache, check via JS
                return await _jsRuntime.InvokeAsync<bool>("tokenStorage.isTokenExpiringSoon");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token expiration");
                return false;
            }
        }
    }
}
