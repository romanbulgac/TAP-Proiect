using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetForUser(Guid userId)
        {
            var notifications = _notificationService.GetNotificationsForUser(userId);
            return Ok(notifications);
        }

        [HttpPost]
        public IActionResult Create([FromBody] NotificationDto dto)
        {
            _notificationService.CreateNotification(dto);
            return Ok();
        }
    }
}
