namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Notifications;

    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService _notificationService)
        {
            this.notificationService = _notificationService;
        }

        public async Task<IActionResult> Index()
        {
            string userId = this.GetUserId();

            AllNotificationsViewModel viewModel = new AllNotificationsViewModel
            {
                UserId = userId,
                Notifications = await this.notificationService.GetAllByUserIdAsync(userId)
            };

            return View(viewModel);
        }
    }
}