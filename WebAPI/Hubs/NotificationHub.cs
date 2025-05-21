// In WebAPI/Hubs/NotificationHub.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Security.Claims;
using BusinessLayer.DTOs;

namespace WebAPI.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // Add to user-specific group for targeted notifications
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                
                // Add to role-based groups
                if (!string.IsNullOrEmpty(userRole))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Role_{userRole}");
                }
                
                _logger.LogInformation("User {UserId} connected to NotificationHub", userId);
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                _logger.LogInformation("User {UserId} disconnected from NotificationHub", userId);
            }
            
            await base.OnDisconnectedAsync(exception);
        }
    }
}