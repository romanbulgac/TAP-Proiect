using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Tests.WebAPI.IntegrationTests
{
    public class ConsultationsControllerTests : IClassFixture<WebApplicationFactory<WebAPI.Program>>
    {
        private readonly WebApplicationFactory<WebAPI.Program> _factory;

        public ConsultationsControllerTests(WebApplicationFactory<WebAPI.Program> factory)
        {
            _factory = factory;
        }

        private async Task<string> GetTeacherTokenAsync(HttpClient client)
        {
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Email = "emma.johnson@mathconsult.com",
                Password = "Teacher123!"
            });
            
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResultDto>();
            return loginResult?.Token ?? string.Empty;
        }

        private async Task<string> GetStudentTokenAsync(HttpClient client)
        {
            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Email = "alex.brown@mathconsult.com",
                Password = "Student123!"
            });
            
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResultDto>();
            return loginResult?.Token ?? string.Empty;
        }

        [Fact]
        public async Task CreateConsultation_AsTeacher_ReturnsCreated()
        {
            // Arrange
            var client = _factory.CreateClient();
            var teacherToken = await GetTeacherTokenAsync(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", teacherToken);
            
            var newConsultation = new ConsultationDto
            {
                Topic = "Test Consultation",
                Description = "This is a test consultation",
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                DurationMinutes = 60
            };
            
            // Act
            var response = await client.PostAsJsonAsync("/api/consultations", newConsultation);
            
            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateConsultation_AsStudent_ReturnsForbidden()
        {
            // Arrange
            var client = _factory.CreateClient();
            var studentToken = await GetStudentTokenAsync(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", studentToken);
            
            var newConsultation = new ConsultationDto
            {
                Topic = "Test Consultation",
                Description = "This should not be allowed for students",
                ScheduledAt = DateTime.UtcNow.AddDays(1),
                DurationMinutes = 60
            };
            
            // Act
            var response = await client.PostAsJsonAsync("/api/consultations", newConsultation);
            
            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetAllConsultations_WithoutAuth_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();
            
            // Act
            var response = await client.GetAsync("/api/consultations");
            
            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}
