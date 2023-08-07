namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Administration.Users;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class UserController : AdminController
    {
        private const int UsersPerPage = 2;

        private readonly IUserService userService;
        private readonly INotificationService notificationService;

        public UserController(
            IUserService _userService,
            INotificationService _notificationService)
        {
            this.userService = _userService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Active(int page = 1)
        {
            int usersCount = await this.userService.GetActiveOrDeletedUsersCountAsync(false);

            int totalPages = (int)Math.Ceiling((double)usersCount / UsersPerPage);
            totalPages = totalPages == 0 ? 1 : totalPages;

            AllUsersViewModel allUsersViewModel = new AllUsersViewModel
            {
                Users = await this.userService
                .GetActiveOrDeletedForAdministrationAsync(false, (page - 1) * UsersPerPage, UsersPerPage),
                CurrentPage = page,
                PagesCount = totalPages
            };

            return this.View(allUsersViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Deleted(int page = 1)
        {
            int usersCount = await this.userService.GetActiveOrDeletedUsersCountAsync(true);

            int totalPages = (int)Math.Ceiling((double)usersCount / UsersPerPage);
            totalPages = totalPages == 0 ? 1 : totalPages;

            AllUsersViewModel allUsersViewModel = new AllUsersViewModel
            {
                Users = await this.userService.GetActiveOrDeletedForAdministrationAsync(true, (page - 1) * UsersPerPage, UsersPerPage),
                CurrentPage = page,
                PagesCount = totalPages
            };

            return this.View(allUsersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            try
            {
                ApplicationUser userToDelete = await this.userService.GetUserByIdAsync(userId);

                if (userToDelete == null)
                {
                    this.TempData[ErrorMessage] = "User does NOT exists!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                if (!this.User.IsInRole("Admin"))
                {
                    this.TempData[ErrorMessage] = "You are NOT an admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                await this.userService.DeleteUserAsync(userId);


                this.TempData[SuccessMessage] = "You deleted a user!";

                await this.notificationService.CreateNotificationAsync(
                    $"You succesfully deleted user: {userToDelete!.Email}!",
                    $"/Admin/User/All",
                    this.GetUserId());
                
                return this.RedirectToAction(nameof(Active));
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }
    }
}
