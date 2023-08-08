namespace MyGymWorld.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Server.IIS.Core;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Administration.Managers;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class ManagerController : AdminController
    {
        private readonly IUserService userService;
        private readonly IManagerService managerService;

        public ManagerController(
            IUserService _userService,
            IManagerService _managerService,
            INotificationService _notificationService)
        {
            this.userService = _userService;
            this.managerService = _managerService;
        }

        [HttpGet]
        public async Task<IActionResult> Requests()
        {
            AllRequestsViewModel allRequests = new AllRequestsViewModel
            {
                AdminId = this.GetUserId(),
                Requests = await this.managerService.GetAllNotApprovedManagerRequestsAsync()
            };

            return this.View(allRequests);
        }

        [HttpGet]
        public async Task<IActionResult> RequestDetails(string managerId)
        {
            try
            {
                Manager? manager = await this.managerService.GetManagerByIdAsync(managerId);

                if (manager == null)
                {
                    this.TempData[ErrorMessage] = "Such manager does NOT exist!";

                    return this.RedirectToAction(nameof(Requests), new { area = "Admin" });
                }

                ManagerRequestViewModel managerRequestViewModel = await this.managerService.GetSingleManagerRequestByManagerIdAsync(managerId);

                return this.View(managerRequestViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction(nameof(Requests), new { area = "Admin" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApproveManager(string managerId)
        {
            Manager? manager = await this.managerService.GetManagerForApprovalAndRejectionAsync(managerId);

            if (manager == null)
            {
                this.TempData[ErrorMessage] = "User with this Id does not exists!";

                return this.RedirectToAction("RequestDetails", "Manager", new { managerId });
            }

            try
            {
                string adminId = this.GetUserId();

                await this.managerService.ApproveManagerAsync(managerId, adminId);

                this.TempData[SuccessMessage] = "You succesfully approved a manager request!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction("Active","User");
        }

        [HttpGet]
        public async Task<IActionResult> RejectManager(string managerId)
        {
            Manager? manager = await this.managerService.GetManagerForApprovalAndRejectionAsync(managerId);

            if (manager == null)
            {
                this.TempData[ErrorMessage] = "User with this Id does not exists!";

                return this.RedirectToAction("RequestDetails", "Manager", new { managerId });
            }

            try
            {
                string adminId = this.GetUserId();

                await this.managerService.RejectManagerAsync(managerId, adminId);

                this.TempData[SuccessMessage] = "You succesfully rejected a Manager!";
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }

            return this.RedirectToAction("Active", "User");
        }
    }
}
