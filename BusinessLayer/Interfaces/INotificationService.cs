namespace BusinessLayer.Interfaces
{
    public interface INotificationService
    {
        void CreateNotification(DTOs.NotificationDto dto);
        IEnumerable<DTOs.NotificationDto> GetNotificationsForUser(Guid userId);
        void SendInAppNotification(string message, Guid userId);
    }
}
