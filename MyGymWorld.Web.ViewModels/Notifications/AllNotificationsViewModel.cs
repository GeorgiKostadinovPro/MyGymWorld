namespace MyGymWorld.Web.ViewModels.Notifications
{
    using System.Collections.Generic;

    public class AllNotificationsViewModel
    {
        public AllNotificationsViewModel()
        {
            this.Notifications = new HashSet<NotificationViewModel>();
        }
        public string UserId { get; set; } = null!;
        
        public IEnumerable<NotificationViewModel> Notifications { get; set; }
    }
}
