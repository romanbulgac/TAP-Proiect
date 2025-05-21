// filepath: /Users/romanbulgac/Documents/University/Semestru VI/TAP/Proiect/BusinessLayer/Implementations/NotificationService.cs
using BusinessLayer.Interfaces;
using BusinessLayer.DTOs;
using DataAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Linq;

namespace BusinessLayer.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IRealtimeNotifier _realtimeNotifier; // Changed

        public NotificationService(MyDbContext context, IMapper mapper, IRealtimeNotifier realtimeNotifier) // Updated
        {
            _context = context;
            _mapper = mapper;
            _realtimeNotifier = realtimeNotifier; // Store injected notifier
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationDto dto)
        {
            var entity = new Notification
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                UserId = dto.UserId,
                Title = dto.Title,
                Message = dto.Message,
                NotificationType = dto.Type,
                ConsultationId = dto.ConsultationId,
                IsRead = dto.IsRead,
                SentAt = dto.SentAt,
                Link = dto.Link,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(entity);
            await _context.SaveChangesAsync();

            var createdDto = _mapper.Map<NotificationDto>(entity);

            // Send real-time notification to the specific user via the interface
            if (createdDto.UserId != Guid.Empty)
            {
                await _realtimeNotifier.SendNotificationToUserAsync(createdDto.UserId.ToString(), createdDto);
            }
            
            return createdDto;
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Include(n => n.User)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task SendInAppNotificationAsync(string message, Guid userId)
        {
            var notificationEntity = new Notification // Renamed from 'notification' to avoid conflict
            {
                UserId = userId,
                Title = "System Notification",
                Message = message,
                NotificationType = "System",
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notificationEntity); // Use renamed variable
            await _context.SaveChangesAsync();

            var notificationDto = _mapper.Map<NotificationDto>(notificationEntity); // Use renamed variable

            // Send real-time notification via the interface
            if (userId != Guid.Empty)
            {
                 await _realtimeNotifier.SendNotificationToUserAsync(userId.ToString(), notificationDto);
            }
        }

        public async Task MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SendBulkNotificationsAsync(string message, IEnumerable<Guid> userIds)
        {
            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = "Bulk Notification",
                Message = message,
                NotificationType = "Bulk",
                IsRead = false,
                SentAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                // Instead of _context.Notifications.Remove(notification);
                notification.IsDeleted = true;
                notification.DeletedAt = DateTime.UtcNow;
                // _context.Notifications.Update(notification); // EF Core tracks changes, explicit Update not always needed
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
                
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.UpdatedAt = DateTime.UtcNow;
            }
            
            await _context.SaveChangesAsync();
            
            // If we have a realtime notifier, we could notify the user that all notifications have been marked as read
            // This is optional but could be useful for UI updates
            // await _realtimeNotifier.NotifyUserOfAllNotificationsReadAsync(userId.ToString());
        }
    }
}
