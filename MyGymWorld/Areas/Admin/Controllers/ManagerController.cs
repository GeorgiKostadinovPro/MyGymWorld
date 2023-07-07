namespace MyGymWorld.Web.Areas.Administration.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Administration.Managers;

    public class ManagerController : AdminController
    {
        private readonly IUserService userService;
        private readonly IManagerService managerService;

        public ManagerController(
            IUserService _userService,
            IManagerService _managerService)
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
            ManagerRequestViewModel managerRequestViewModel = await this.managerService.GetSingleManagerRequestByManagerIdAsync(managerId);

            return this.View(managerRequestViewModel);
        }

        public async Task<IActionResult> ApproveManager(string managerId)
        {
            return this.RedirectToAction(nameof(RequestDetails), new { managerId = managerId });
        }
    }
}
