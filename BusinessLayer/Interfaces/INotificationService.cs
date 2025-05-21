using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer.DTOs;

namespace BusinessLayer.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(NotificationDto dto);
        Task<IEnumerable<NotificationDto>> GetNotificationsForUserAsync(Guid userId);
        Task SendInAppNotificationAsync(string message, Guid userId);
        Task MarkNotificationAsReadAsync(Guid notificationId);
        Task SendBulkNotificationsAsync(string message, IEnumerable<Guid> userIds);
        Task DeleteNotificationAsync(Guid notificationId);
        Task<int> GetUnreadNotificationCountAsync(Guid userId);
        Task MarkAllNotificationsAsReadAsync(Guid userId);
    }
}
