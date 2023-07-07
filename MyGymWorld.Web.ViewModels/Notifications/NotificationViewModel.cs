namespace MyGymWorld.Web.ViewModels.Notifications
{
    using System;

    public class NotificationViewModel
    {
        public string Id { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string? Url { get; set; }

        public bool IsRead { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserId { get; set; } = null!;
    }
}
