namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Notifications;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class NotificationController : BaseController
    {
        private readonly int NotificationsPerPage = 5;

        private readonly INotificationService notificationService;
        private readonly IUserService userService;

        public NotificationController
            (INotificationService _notificationService, 
            IUserService userService)
        {
            this.notificationService = _notificationService;

            this.userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            string userId = this.GetUserId();

            int count = await this.notificationService.GetActiveNotificationsCountByUserIdAsync(userId);

            int totalPages = (int)Math.Ceiling((double)count / NotificationsPerPage);
            totalPages = totalPages == 0 ? 1 : totalPages;

            AllNotificationsViewModel viewModel = new AllNotificationsViewModel
            {
                UserId = userId,
                CurrentPage = page,
                PagesCount = totalPages,
                Notifications = await this.notificationService.GetActiveNotificationsByUserIdAsync(userId, (page - 1) * NotificationsPerPage, NotificationsPerPage)
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
                await this.notificationService.ReadNotificationAsync(notificationId);

                this.TempData[SuccessMessage] = "You read notification!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Clear()
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                await this.notificationService.DeleteAllNotificationsByUserIdAsync(userId);

                this.TempData[SuccessMessage] = "You cleared all notifications!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReadAll()
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user == null)
                {
                    this.TempData[ErrorMessage] = "Such user does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                await this.notificationService.ReadAllNotificationsByUserIdAsync(userId);

                this.TempData[SuccessMessage] = "You read all notifications!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction(nameof(Index));
        }
    }
}