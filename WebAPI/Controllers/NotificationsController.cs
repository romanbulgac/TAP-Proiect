using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BusinessLayer.DTOs;
using System.Collections.Generic;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // All endpoints require authentication by default
    public class NotificationsController : ControllerBase // Renamed from NotificationController for consistency
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new InvalidOperationException("User ID not found or invalid in token.");
            }
            return userId;
        }

        [HttpGet("user")] // Get notifications for the currently authenticated user
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications()
        {
            var userId = GetCurrentUserId();
            var notifications = await _notificationService.GetNotificationsForUserAsync(userId);
            return Ok(notifications);
        }
        
        [HttpGet("user/unread-count")]
        public async Task<ActionResult<int>> GetUnreadNotificationCount()
        {
            var userId = GetCurrentUserId();
            var count = await _notificationService.GetUnreadNotificationCountAsync(userId);
            return Ok(count);
        }


        [HttpPost("notify-inapp")] // This seems like a utility endpoint, perhaps for admin or system use
        [Authorize(Roles = "Administrator")] // Example: Only Admin can trigger this directly
        public async Task<IActionResult> NotifyInApp([FromBody] InAppNotificationRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _notificationService.SendInAppNotificationAsync(request.Message, request.UserId);
            return Ok(new { message = "In-app notification sent." });
        }

        [HttpPut("{notificationId}/mark-read")]
        public async Task<IActionResult> MarkNotificationAsRead(Guid notificationId)
        {
            var userId = GetCurrentUserId();
            // Optional: Verify the notification belongs to the user before marking as read
            // var notification = await _notificationService.GetNotificationByIdAsync(notificationId); // Needs GetNotificationByIdAsync
            // if (notification == null || notification.UserId != userId) return Forbid();
            await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return NoContent();
        }
        
        [HttpPut("mark-all-read")]
        public async Task<IActionResult> MarkAllNotificationsAsRead()
        {
            var userId = GetCurrentUserId();
            await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return NoContent();
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var userId = GetCurrentUserId();
            // Optional: Verify ownership before deleting
            await _notificationService.DeleteNotificationAsync(notificationId); // This performs soft delete
            return NoContent();
        }
    }

    // Helper DTO for the specific endpoint
    public class InAppNotificationRequestDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        public string Message { get; set; } = string.Empty;
        [System.ComponentModel.DataAnnotations.Required]
        public Guid UserId { get; set; }
    }
}
