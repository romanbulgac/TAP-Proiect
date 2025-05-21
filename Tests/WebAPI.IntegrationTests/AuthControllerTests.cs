using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.WebAPI.IntegrationTests
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<WebAPI.Program>>
    {
        private readonly WebApplicationFactory<WebAPI.Program> _factory;

        public AuthControllerTests(WebApplicationFactory<WebAPI.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsTokenAndUserInfo()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginRequest = new LoginRequestDto
            {
                Email = "emma.johnson@mathconsult.com",
                Password = "Teacher123!"
            };
            
            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            var content = await response.Content.ReadFromJsonAsync<AuthenticationResultDto>();
            
            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(content);
            Assert.True(content.IsSuccess);
            Assert.NotNull(content.Token);
            Assert.NotNull(content.User);
            Assert.Equal("emma.johnson@mathconsult.com", content.User.Email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginRequest = new LoginRequestDto
            {
                Email = "invalid@mathconsult.com",
                Password = "WrongPassword123!"
            };
            
            // Act
            var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
            
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AccessProtectedEndpoint_WithoutToken_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            
            // Act
            var response = await client.GetAsync("/api/notifications/user");
            
            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
