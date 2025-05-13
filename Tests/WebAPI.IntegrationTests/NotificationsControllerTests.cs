using Xunit;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Tests.WebAPI.IntegrationTests
{
    public class NotificationsControllerTests
        : IClassFixture<WebApplicationFactory<WebAPI.Program>>
    {
        private readonly WebApplicationFactory<WebAPI.Program> _factory;

        public NotificationsControllerTests(WebApplicationFactory<WebAPI.Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetForUser_ReturnsOk()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/Notifications/00000000-0000-0000-0000-000000000000");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
