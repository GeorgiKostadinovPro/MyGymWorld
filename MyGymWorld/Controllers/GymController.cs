namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Gyms;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class GymController : BaseController
    {
        private readonly IGymService gymService;
        private readonly IUserService userService;  
        private readonly IManagerService managerService;
        private readonly ICommentService commentService;
        private readonly INotificationService notificationService;

        public GymController(
            IGymService _gymService, 
            IUserService _userService,  
            IManagerService _managerService,
            INotificationService _notificationService)
        {
            this.gymService = _gymService;
            this.userService = _userService;
            this.managerService = _managerService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllGymsQueryModel queryModel)
        {
            try
            {
                AllGymsFilteredAndPagedViewModel allGymsFilteredAndPagedViewModel = new AllGymsFilteredAndPagedViewModel
                {
                    TotalGymsCount = await this.gymService.GetActiveGymsCountAsync(),
                    Gyms = await this.gymService.GetAllActiveGymsFilteredAndPagedAsync(queryModel)
                };

                queryModel.GymTypes = this.gymService.GetAllGymTypes();
                queryModel.TotalGymsCount = allGymsFilteredAndPagedViewModel.TotalGymsCount;
                queryModel.Gyms = allGymsFilteredAndPagedViewModel.Gyms;
                
                return this.View(queryModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string gymId)
        {
            try
            {
                bool doesGymExist = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

                if (!doesGymExist)
                {
                    this.TempData[ErrorMessage] = "Such Gym does NOT exists!";

                    return this.NotFound();
                }

                GymDetailsViewModel gymDetailsViewModel = await this.gymService.GetGymDetailsByIdAsync(gymId);

                return this.View(gymDetailsViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction(nameof(All));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Join(string gymId)
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                bool doesGymExist = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

                if (!doesGymExist)
                {
                    this.TempData[ErrorMessage] = "Such Gym does NOT exists!";

                    return this.NotFound();
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                Manager? manager = await this.managerService.GetManagerByUserIdAsync(this.GetUserId());

                if (manager != null)
                {
                    bool isUserManagerOfThisGym = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, manager.Id.ToString());

                    if (isUserManagerOfThisGym)
                    {
                        this.TempData[ErrorMessage] = "You are the manager of this gym! You can NOT join it!";

                        return this.RedirectToAction(nameof(Details), new { gymId = gymId });
                    }
                }

                await this.gymService.AddGymToUserAsync(gymId, user.Id.ToString());

                await this.notificationService.CreateNotificationAsync(
                    $"You joined {gym.Name}!",
                    $"/Gym/Details?gymId={gymId}",
                    user.Id.ToString());

                this.TempData[SuccessMessage] = "You successfully joined the gym!";

                return this.RedirectToAction(nameof(Joined));
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction(nameof(Details), new { gymId = gymId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction(nameof(Details), new { gymId = gymId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Leave(string gymId)
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.IsDeleted == true)
                {
                    this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                    return this.RedirectToAction("Index", "Home", new { area = "" });
                }

                bool doesGymExist = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

                if (!doesGymExist)
                {
                    this.TempData[ErrorMessage] = "Such Gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home", new { area = "", statusCode = 404 });
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                Manager? manager = await this.managerService.GetManagerByUserIdAsync(this.GetUserId());

                if (manager != null)
                {
                    bool isUserManagerOfThisGym = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, manager.Id.ToString());

                    if (isUserManagerOfThisGym)
                    {
                        this.TempData[ErrorMessage] = "You are the manager of this gym! You can NOT join it!";

                        return this.RedirectToAction(nameof(Details), new { gymId = gymId });
                    }
                }

                await this.gymService.RemoveGymFromUserAsync(gymId, user.Id.ToString());

                await this.notificationService.CreateNotificationAsync(
                  $"You left {gym.Name}!",
                  $"/Gym/Details?gymId={gymId}",
                  user.Id.ToString());

                this.TempData[SuccessMessage] = "You successfully left the gym!";
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }
            
            return this.RedirectToAction(nameof(Details), new { gymId = gymId });
        }

        [HttpGet]
        public async Task<IActionResult> Joined([FromQuery] AllUserJoinedGymsQueryModel queryModel)
        {
            try
            {
				string userId = this.GetUserId();

				ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

				if (user.IsDeleted == true)
				{
					this.TempData[ErrorMessage] = "You were deleted by the Admin!";

					return this.RedirectToAction("Index", "Home", new { area = "" });
				}

				AllUserJoinedGymsFilteredAndPagedViewModel allGymsFilteredAndPagedViewModel = new AllUserJoinedGymsFilteredAndPagedViewModel
                {
                    UserId = userId,
                    TotalGymsCount = await this.gymService.GetAllUserJoinedGymsCountAsync(userId),
                    Gyms = await this.gymService.GetAllUserJoinedGymsFilteredAndPagedAsync(userId, queryModel)
                };

                queryModel.UserId = userId;
                queryModel.GymTypes = this.gymService.GetAllGymTypes();
                queryModel.TotalGymsCount = allGymsFilteredAndPagedViewModel.TotalGymsCount;
                queryModel.Gyms = allGymsFilteredAndPagedViewModel.Gyms;

                return this.View(queryModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Home", "Index");
            }
        }
    }
}