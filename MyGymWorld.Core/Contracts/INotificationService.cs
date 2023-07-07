namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Notifications;
    using System.Threading.Tasks;

    public interface INotificationService
    {
        Task<Notification?> CreateNotificationAsync(string userId ,string content, string url);

        Task<Notification> DeleteNotificationAsync(string notificationId);

        Task<Notification> ReadNotificationByIdAsync(string notificationId);

        Task<IEnumerable<NotificationViewModel>> GetFilteredNotificationsByUserIdAsync(string userId, bool isRead);

        Task<IEnumerable<NotificationViewModel>> GetAllByUserIdAsync(string userId);

        Task<int> GetUnReadNotificationsCountByUserIdAsync(string userId);

        Task<int> GetAllNotificationsCountByUserIdAsync(string userId);
    }
}
