using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        // Dependency injection for INotificationService
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("notify-inapp")]
        public IActionResult NotifyInApp(string message, Guid userId)
        {
            // Logic to send in-app notification
            _notificationService.SendInAppNotification(message, userId);
            return Ok();
        }

        // Other endpoints for notifications can be added here
    }
}