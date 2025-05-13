using BlazorClient.Models;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorClient.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<LoginResult> Login(LoginRequest loginRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", loginRequest); // Adjust API endpoint
            if (response.IsSuccessStatusCode)
            {
                var loginResult = await response.Content.ReadFromJsonAsync<LoginResult>();
                if (loginResult != null && loginResult.Successful && !string.IsNullOrEmpty(loginResult.Token))
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", loginResult.Token);
                    ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated(loginResult.Token);
                    return loginResult;
                }
                return loginResult ?? new LoginResult { Successful = false, Error = "Failed to deserialize login result." };
            }
            return new LoginResult { Successful = false, Error = "Login failed: " + response.ReasonPhrase };
        }

        public async Task Logout()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            ((CustomAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }

        public async Task<bool> Register(RegisterRequest registerRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/register", registerRequest); // Adjust API endpoint
            return response.IsSuccessStatusCode;
        }
    }

    public interface IAuthService
    {
        Task<LoginResult> Login(LoginRequest loginRequest);
        Task Logout();
        Task<bool> Register(RegisterRequest registerRequest);
    }
}
