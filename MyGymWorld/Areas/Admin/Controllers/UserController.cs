﻿namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using System.Runtime.CompilerServices;
    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class UserController : AdminController
    {
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
        public async Task<IActionResult> All()
        {
            AllUsersViewModel allUsersViewModel = new AllUsersViewModel
            {
                Users = await this.userService.GetAllForAdministrationAsync(),
            };

            return this.View(allUsersViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            ApplicationUser userToDelete = await this.userService.GetUserByIdAsync(userId);

            if (userToDelete == null)
            {
                this.TempData[ErrorMessage] = "User does NOT exists!";
            }

            try
            {
                await this.userService.DeleteUserAsync(userId);

                await this.notificationService.CreateNotificationAsync(
                    $"You succesfully deleted user: {userToDelete!.Email}!",
                    $"/Admin/User/All",
                    this.GetUserId());
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction(nameof(All));
        }
    }
}
