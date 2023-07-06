namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Managers;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class ManagerController : BaseController
    {
        private readonly IManagerService managerService;
        private readonly IRoleService roleService;

        public ManagerController(IManagerService _managerService, IRoleService _roleService)
        {
            this.managerService = _managerService;
            this.roleService = _roleService;
        }

        [HttpGet]
        public async Task<IActionResult> BecomeManager()
        {
            string userId = this.GetUserId();

            bool isManager = await this.managerService.CheckIfUserIsAManagerAsync(userId);

            if (isManager)
            {
                this.TempData[ErrorMessage] = "You are already a manager!";

                return this.RedirectToAction("Index", "Home");
            }

            BecomeManagerInputModel becomeManagerInputModel = await this.managerService.GetUserToBecomeManagerByIdAsync(userId);

            return this.View(becomeManagerInputModel);
        }

        [HttpPost]
        public async Task<IActionResult> BecomeManager(string id, BecomeManagerInputModel becomeManagerInputModel)
        {
            becomeManagerInputModel.ManagerTypes = this.managerService.GetAllManagerTypes();

            if (!this.ModelState.IsValid)
            {
                return this.View(becomeManagerInputModel);
            }
            
            if (becomeManagerInputModel.ManagerType == "None")
            {
                this.ModelState.AddModelError("ManagerType", "You should choose a manager type!");

                return this.View(becomeManagerInputModel);
            }

            bool isThereManagerWithPhoneNumber = await this.managerService.CheckIfManagerExistsNyPhoneNumberAsync(becomeManagerInputModel.PhoneNumber);

            if (isThereManagerWithPhoneNumber)
            {
                this.TempData[ErrorMessage] = "There is already a manager with this phone!";

                return this.View(becomeManagerInputModel);
            }

            try
            {
                await this.managerService.CreateManagerAsync(id, becomeManagerInputModel);
                await this.roleService.AddRoleToUserAsync(id, ApplicationRoleConstants.ManagerRoleName);

                this.TempData[SuccessMessage] = "You are now a manager!";

                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.View(becomeManagerInputModel);
            }
        }
    }
}
