namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Notifications;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class NotificationController : BaseController
    {
        private readonly int NotificationsPerPage = 5;

        private readonly INotificationService notificationService;

        public NotificationController(INotificationService _notificationService)
        {
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            string userId = this.GetUserId();

            int count = await this.notificationService.GetAllNotificationsCountByUserIdAsync(userId);

            int totalPages = (int)Math.Ceiling((double)count / NotificationsPerPage);
            totalPages = totalPages == 0 ? 1 : totalPages;

            AllNotificationsViewModel viewModel = new AllNotificationsViewModel
            {
                UserId = userId,
                CurrentPage = page,
                PagesCount = totalPages,
                Notifications = await this.notificationService.GetAllNotificationsByUserIdAsync(userId, (page - 1) * NotificationsPerPage, NotificationsPerPage)
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

        [HttpPost]
        public async Task<IActionResult> Read(string notificationId)
        {
            try
            {
                await this.notificationService.ReadNotificationByIdAsync(notificationId);

                this.TempData[SuccessMessage] = "You read notification!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction(nameof(Index));
        }
    }
}