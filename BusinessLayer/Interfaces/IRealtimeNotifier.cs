using System.Threading.Tasks;
using BusinessLayer.DTOs; // For NotificationDto or a generic payload

namespace BusinessLayer.Interfaces
{
    public interface IRealtimeNotifier
    {
        Task SendNotificationToUserAsync(string userId, NotificationDto notificationPayload);
        // Add other methods if needed, e.g., SendToGroupAsync, SendToAllAsync
    }
}
