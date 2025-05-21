// In BlazorClient/Services/NotificationService.cs
using BlazorClient.Models;
using DataAccess.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorClient.Services
{
    public interface INotificationService
    {
        event Action<Notification> NotificationReceived;
        Task<IEnumerable<Notification>> GetNotificationsAsync();
        Task<bool> MarkAsReadAsync(Guid notificationId);
        Task<bool> MarkAllAsReadAsync();
        Task InitializeHubConnectionAsync();
        Task CloseHubConnectionAsync();
    }

    public class NotificationService : INotificationService, IAsyncDisposable
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJSRuntime _jsRuntime;
        private readonly ILogger<NotificationService> _logger;
        private readonly AuthenticationStateProvider _authStateProvider;
        private HubConnection? _hubConnection;
        private bool _isConnectionInitialized = false;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(1, 1);

        public event Action<Notification>? NotificationReceived;

        public NotificationService(
            IHttpClientFactory httpClientFactory,
            IJSRuntime jsRuntime,
            ILogger<NotificationService> logger,
            AuthenticationStateProvider authStateProvider)
        {
            _httpClientFactory = httpClientFactory;
            _jsRuntime = jsRuntime;
            _logger = logger;
            _authStateProvider = authStateProvider;
            
            // Listen for authentication state changes
            _authStateProvider.AuthenticationStateChanged += AuthStateChanged;
        }

        private async void AuthStateChanged(Task<AuthenticationState> task)
        {
            var authState = await task;
            
            if (authState.User.Identity?.IsAuthenticated == true)
            {
                // User authenticated, start connection if not already connected
                await InitializeHubConnectionAsync();
            }
            else
            {
                // User logged out, close connection
                await CloseHubConnectionAsync();
            }
        }

        public async Task InitializeHubConnectionAsync()
        {
            try
            {
                await _connectionLock.WaitAsync();
                
                if (_isConnectionInitialized)
                    return;
                
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                if (authState.User.Identity?.IsAuthenticated != true)
                {
                    _logger.LogInformation("Not initializing SignalR - user not authenticated");
                    return;
                }

                // Get the JWT token
                var token = await _jsRuntime.InvokeAsync<string>("tokenStorage.getAccessToken");
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("Cannot initialize SignalR - no token available");
                    return;
                }

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl("http://localhost:5217/notificationHub", options =>
                    {
                        options.AccessTokenProvider = () => Task.FromResult(token)!;
                    })
                    .WithAutomaticReconnect()
                    .Build();

                // Register for notification messages
                _hubConnection.On<Notification>("ReceiveNotification", notification =>
                {
                    NotificationReceived?.Invoke(notification);
                });

                // Start the connection
                await _hubConnection.StartAsync();
                _isConnectionInitialized = true;
                _logger.LogInformation("SignalR connection established");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing SignalR connection");
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task CloseHubConnectionAsync()
        {
            try
            {
                await _connectionLock.WaitAsync();
                
                if (_hubConnection != null && _isConnectionInitialized)
                {
                    await _hubConnection.StopAsync();
                    await _hubConnection.DisposeAsync();
                    _hubConnection = null;
                    _isConnectionInitialized = false;
                    _logger.LogInformation("SignalR connection closed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing SignalR connection");
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public async Task<IEnumerable<Notification>> GetNotificationsAsync()
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var result = await client.GetFromJsonAsync<IEnumerable<Notification>>("api/Notifications/user");
                return result ?? new List<Notification>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching notifications");
                return new List<Notification>();
            }
        }

        public async Task<bool> MarkAsReadAsync(Guid notificationId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.PutAsync($"api/Notifications/{notificationId}/mark-read", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return false;
            }
        }

        public async Task<bool> MarkAllAsReadAsync()
        {
            try
            {
                // Using the correct endpoint
                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.PutAsync("api/Notifications/mark-all-read", null);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Failed to mark all notifications as read: {response.StatusCode}");
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return false;
            }
        }

        public async ValueTask DisposeAsync()
        {
            // Unregister from authentication state changes
            _authStateProvider.AuthenticationStateChanged -= AuthStateChanged;
            
            // Close the SignalR connection
            await CloseHubConnectionAsync();
            
            // Dispose the semaphore
            _connectionLock.Dispose();
        }
    }
}