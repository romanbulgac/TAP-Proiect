using Xunit;
using Bunit;
using Moq;
using BlazorClient.Services;
using BlazorClient.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System;
using Blazored.LocalStorage;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlazorClient.Tests
{
    public class AuthenticationTests : TestContext
    {
        private readonly Mock<HttpClient> _mockHttpClient;
        private readonly Mock<ILocalStorageService> _mockLocalStorage;

        public AuthenticationTests()
        {
            _mockHttpClient = new Mock<HttpClient>();
            _mockLocalStorage = new Mock<ILocalStorageService>();

            // bUnit specific services for testing components that use AuthenticationStateProvider
            Services.AddSingleton<HttpClient>(_mockHttpClient.Object);
            Services.AddScoped(sp => _mockLocalStorage.Object); // Register the mock
            Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            Services.AddAuthorizationCore(); // Required for AuthorizeView and other auth components
        }

        // Helper to generate a simple JWT for testing purposes
        private string GenerateTestJwt(IEnumerable<Claim> claims, DateTime expiry)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourTestSecretKeyNeedsToBeLongEnoughAndSecure"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "test-issuer",
                audience: "test-audience",
                claims: claims,
                expires: expiry,
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Fact]
        public async Task InitialState_UserShouldBeNullAndNotAuthenticated()
        {
            // Arrange
            var authProvider = Services.GetRequiredService<AuthenticationStateProvider>() as AuthStateProvider;
            
            // Simulate that no token is found in local storage
            _mockLocalStorage.Setup(ls => ls.GetItemAsStringAsync("authToken", It.IsAny<CancellationToken>()))
                            .ReturnsAsync((string?)null);

            // Act
            var authState = await authProvider!.GetAuthenticationStateAsync();

            // Assert
            Assert.NotNull(authState);
            Assert.False(authState.User.Identity?.IsAuthenticated);
            Assert.Null(authProvider.CurrentUser);
            Assert.Null(authProvider.JwtToken);
        }

        [Fact]
        public async Task MarkUserAsAuthenticatedAsync_WithValidToken_ShouldUpdateStateAndCurrentUser()
        {
            // Arrange
            var authProvider = Services.GetRequiredService<AuthenticationStateProvider>() as AuthStateProvider;
            var userId = Guid.NewGuid();
            var userEmail = "test@example.com";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, userEmail),
                new Claim("firstName", "Test"),
                new Claim("lastName", "User"),
                new Claim(ClaimTypes.Role, "User")
            };
            var jwtToken = GenerateTestJwt(claims, DateTime.UtcNow.AddHours(1));

            var authStateChangedTcs = new TaskCompletionSource<AuthenticationState>();
            authProvider!.AuthenticationStateChanged += (task) => 
            {
                authStateChangedTcs.TrySetResult(task.Result); // Use TrySetResult to avoid issues if event fires multiple times
            };

            _mockLocalStorage.Setup(ls => ls.SetItemAsStringAsync("authToken", jwtToken, It.IsAny<CancellationToken>()))
                            .Returns(ValueTask.CompletedTask);

            // Act
            await authProvider.MarkUserAsAuthenticatedAsync(jwtToken);
            var notifiedAuthState = await authStateChangedTcs.Task; 

            // Assert
            Assert.True(notifiedAuthState.User.Identity?.IsAuthenticated);
            Assert.Equal(userEmail, notifiedAuthState.User.FindFirst(ClaimTypes.Email)?.Value);
            Assert.NotNull(authProvider.CurrentUser);
            Assert.Equal(userId, authProvider.CurrentUser?.Id);
            Assert.Equal(userEmail, authProvider.CurrentUser?.Email);
            Assert.Equal("Test", authProvider.CurrentUser?.Name);
            Assert.Equal(jwtToken, authProvider.JwtToken);
            _mockLocalStorage.Verify(ls => ls.SetItemAsStringAsync("authToken", jwtToken, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkUserAsLoggedOutAsync_ShouldClearStateAndCurrentUser()
        {
            // Arrange
            var authProvider = Services.GetRequiredService<AuthenticationStateProvider>() as AuthStateProvider;
            
            // Simulate a logged-in state first
            var initialUserId = Guid.NewGuid();
            var initialClaims = new List<Claim> { 
                new Claim(ClaimTypes.NameIdentifier, initialUserId.ToString()),
                new Claim(ClaimTypes.Email, "initial@example.com")
            };
            var initialJwt = GenerateTestJwt(initialClaims, DateTime.UtcNow.AddHours(1));
            _mockLocalStorage.Setup(ls => ls.SetItemAsStringAsync("authToken", initialJwt, It.IsAny<CancellationToken>()))
                            .Returns(ValueTask.CompletedTask);
            await authProvider!.MarkUserAsAuthenticatedAsync(initialJwt);
            
            // Setup for GetAuthenticationStateAsync to return the token, so logout has something to clear
            _mockLocalStorage.Setup(ls => ls.GetItemAsStringAsync("authToken", It.IsAny<CancellationToken>()))
                            .ReturnsAsync(initialJwt);
            
            var authStateAfterLogin = await authProvider.GetAuthenticationStateAsync(); // Get current state
            Assert.True(authStateAfterLogin.User.Identity?.IsAuthenticated); // Verify logged in

            var authStateChangedTcs = new TaskCompletionSource<AuthenticationState>();
            authProvider.AuthenticationStateChanged += (task) => 
            {
                authStateChangedTcs.TrySetResult(task.Result); 
            };
            
            _mockLocalStorage.Setup(ls => ls.RemoveItemAsync("authToken", It.IsAny<CancellationToken>()))
                            .Returns(ValueTask.CompletedTask);

            // Act
            await authProvider.MarkUserAsLoggedOutAsync();
            var notifiedAuthState = await authStateChangedTcs.Task; 

            // Assert
            Assert.False(notifiedAuthState.User.Identity?.IsAuthenticated);
            Assert.Null(authProvider.CurrentUser);
            Assert.Null(authProvider.JwtToken);
            _mockLocalStorage.Verify(ls => ls.RemoveItemAsync("authToken", It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
