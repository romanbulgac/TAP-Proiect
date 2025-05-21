using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Hubs; // Assuming NotificationHub is in WebAPI.Hubs
using System.Threading.Tasks;

namespace WebAPI.Services
{
    public class SignalRNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotifier(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendNotificationToUserAsync(string userId, NotificationDto notificationPayload)
        {
            // The group name should match how it's defined in NotificationHub.OnConnectedAsync
            await _hubContext.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", notificationPayload);
        }
    }
}
