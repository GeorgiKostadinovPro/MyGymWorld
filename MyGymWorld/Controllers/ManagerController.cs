namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Web.ViewModels.Managers;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class ManagerController : BaseController
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
        public async Task<IActionResult> BecomeManager()
        {
            try
            {
                string userId = this.GetUserId();

                Manager? manager = await this.managerService.GetManagerByUserIdAsync(userId);

                if (manager != null && manager.IsApproved == true)
                {
                    this.TempData[ErrorMessage] = "You are already a manager!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (manager != null && manager.IsRejected)
                {
                    this.TempData[ErrorMessage] = "You were REJECTED for Manager! You cannot apply again!";

                    return this.RedirectToAction("Index", "Home");
                }

                BecomeManagerInputModel becomeManagerInputModel = await this.userService.GetUserToBecomeManagerByIdAsync(userId);

                return this.View(becomeManagerInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public async Task<IActionResult> BecomeManager(string id, BecomeManagerInputModel becomeManagerInputModel)
        {
            try
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

                bool isThereManagerWithPhoneNumber = await this.managerService.CheckIfManagerExistsByPhoneNumberAsync(becomeManagerInputModel.PhoneNumber);

                if (isThereManagerWithPhoneNumber)
                {
                    this.TempData[ErrorMessage] = "There is already an user with this phone!";

                    return this.View(becomeManagerInputModel);
                }

                bool isManagerTypeValid = Enum.TryParse<ManagerType>(becomeManagerInputModel.ManagerType, true, out ManagerType managerType);

                if (!isManagerTypeValid)
                {
                    throw new InvalidOperationException(ExceptionConstants.ManagerErrors.InvalidManagerType);
                }

                await this.managerService.CreateManagerAsync(id, becomeManagerInputModel);

                this.TempData[InformationMessage] = "Manager will aprove you soon!";

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
