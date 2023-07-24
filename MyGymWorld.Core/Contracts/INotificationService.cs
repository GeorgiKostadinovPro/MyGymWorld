namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Notifications;
    using System.Threading.Tasks;

    public interface INotificationService
    {
        Task<Notification?> CreateNotificationAsync(string content, string url, string userId);

        Task<Notification> DeleteNotificationAsync(string notificationId);

        Task<Notification> ReadNotificationByIdAsync(string notificationId);

        Task<IEnumerable<NotificationViewModel>> GetFilteredNotificationsByUserIdAsync(string userId, bool isRead);

        Task<IEnumerable<NotificationViewModel>> GetAllNotificationsByUserIdAsync(string userId, int skip = 0, int? take = null);

        Task<int> GetUnReadNotificationsCountByUserIdAsync(string userId);

        Task<int> GetAllNotificationsCountByUserIdAsync(string userId);
    }
}
