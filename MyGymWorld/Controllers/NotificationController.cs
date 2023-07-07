namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Notifications;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class NotificationController : BaseController
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService _notificationService)
        {
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string userId = this.GetUserId();

            AllNotificationsViewModel viewModel = new AllNotificationsViewModel
            {
                UserId = userId,
                Notifications = await this.notificationService.GetAllByUserIdAsync(userId)
            };

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string notificationId)
        {
            try
            {
                await this.notificationService.DeleteNotificationAsync(notificationId);

                this.TempData[SuccessMessage] = "You removed notification!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }
           
            return this.RedirectToAction(nameof(Index));
        }
    }
}