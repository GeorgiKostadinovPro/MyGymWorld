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

        public int CurrentPage { get; set; }

        public int PagesCount { get; set; }
        
        public IEnumerable<NotificationViewModel> Notifications { get; set; }
    }
}
