namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class GymController : AdminController
    {
        private const int GymsPerPage = 2;

        private readonly IUserService userService;
        private readonly IGymService gymService;

        public GymController(IUserService _userService, IGymService _gymService)
        {
            this.userService = _userService;
            this.gymService = _gymService;
        }

        [HttpGet]
        public async Task<IActionResult> Active(int page = 1)
        {
            ApplicationUser user = await this.userService.GetUserByIdAsync(this.GetUserId());

            if (user == null)
            {
                this.TempData[ErrorMessage] = "Such user does NOT exists!";

                return this.RedirectToAction("Index", "Home");
            }

            if (!User.IsInRole("Administrator"))
            {
                this.TempData[ErrorMessage] = "You do NOT have rights to open this page!";

                return this.RedirectToAction("Index", "Home");
            }

            int count = await this.gymService.GetActiveOrDeletedGymsCountForAdministrationAsync(false);

            int totalPages = (int)Math.Ceiling((double)count / GymsPerPage);

            AllGymsForManagementViewModel allGymsForManagement = new AllGymsForManagementViewModel
            {
                Gyms = await this.gymService
                .GetActiveOrDeletedForAdministrationAsync(false, (page - 1) * GymsPerPage, GymsPerPage),
                CurrentPage = page,
                PagesCount = totalPages
            };

            return this.View(allGymsForManagement);
        }

        [HttpGet]
        public async Task<IActionResult> Deleted(int page = 1)
        {
            ApplicationUser user = await this.userService.GetUserByIdAsync(this.GetUserId());

            if (user == null)
            {
                this.TempData[ErrorMessage] = "Such user does NOT exists!";

                return this.RedirectToAction("Index", "Home");
            }

            if (!User.IsInRole("Administrator"))
            {
                this.TempData[ErrorMessage] = "You do NOT have rights to open this page!";

                return this.RedirectToAction("Index", "Home");
            }

            int count = await this.gymService.GetActiveOrDeletedGymsCountForAdministrationAsync(true);

            int totalPages = (int)Math.Ceiling((double)count / GymsPerPage);

            AllGymsForManagementViewModel allGymsForManagement = new AllGymsForManagementViewModel
            {
                Gyms = await this.gymService
                .GetActiveOrDeletedForAdministrationAsync(true, (page - 1) * GymsPerPage, GymsPerPage),
                CurrentPage = page,
                PagesCount = totalPages
            };

            return this.View(allGymsForManagement);
        }
    }
}
