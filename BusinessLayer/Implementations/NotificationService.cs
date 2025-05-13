using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess;
using DataAccess.Models;

namespace BusinessLayer.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly MyDbContext _context;

        public NotificationService(MyDbContext context)
        {
            _context = context;
            // ...subscribe to domain events if needed...
        }

        public void CreateNotification(NotificationDto dto)
        {
            var entity = new Notification
            {
                Title = dto.Title,
                Message = dto.Message,
                UserId = dto.UserId
            };
            _context.Notifications.Add(entity);
            _context.SaveChanges();
        }

        public void SendInAppNotification(string message, Guid userId)
        {
            var dto = new NotificationDto
            {
                Title = "In-App Notification",
                Message = message,
                UserId = userId
            };
            CreateNotification(dto);
        }

        public IEnumerable<NotificationDto> GetNotificationsForUser(Guid userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId)
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Message = n.Message,
                    IsRead = n.IsRead,
                    SentAt = n.SentAt
                })
                .ToList();
        }
    }
}
