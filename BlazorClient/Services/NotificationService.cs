using BlazorClient.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BlazorClient.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _httpClient;

        public NotificationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync()
        {
            // Adjust API endpoint as needed, e.g., "api/Notifications/user/{userId}"
            return await _httpClient.GetFromJsonAsync<IEnumerable<Notification>>("api/Notifications") ?? new List<Notification>();
        }
    }

    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetNotificationsAsync();
    }
}
