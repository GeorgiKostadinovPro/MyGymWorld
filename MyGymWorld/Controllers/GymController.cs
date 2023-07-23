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
            AllGymsFilteredAndPagedViewModel allGymsFilteredAndPagedViewModel = new AllGymsFilteredAndPagedViewModel
            {
                TotalGymsCount = await this.gymService.GetActiveGymsCountAsync(),
                Gyms = await this.gymService.GetAllFilteredAndPagedActiveGymsAsync(queryModel)
            };
               
            queryModel.GymTypes = this.gymService.GetAllGymTypes();
            queryModel.TotalGymsCount = allGymsFilteredAndPagedViewModel.TotalGymsCount;
            queryModel.Gyms = allGymsFilteredAndPagedViewModel.Gyms;

            return this.View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string gymId)
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

        [HttpPost]
        public async Task<IActionResult> Join(string gymId)
        {
            bool doesGymExist = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

            if (!doesGymExist)
            {
                this.TempData[ErrorMessage] = "Such Gym does NOT exists!";

                return this.NotFound();
            }

            Gym gym = await this.gymService.GetGymByIdAsync(gymId);

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

            ApplicationUser user = await this.userService.GetUserByIdAsync(this.GetUserId());

            if (user.IsDeleted == true)
            {
                this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                return this.RedirectToAction(nameof(Details), new { gymId = gymId });
            }

            try
            {
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
            bool doesGymExist = await this.gymService.CheckIfGymExistsByIdAsync(gymId);

            if (!doesGymExist)
            {
                this.TempData[ErrorMessage] = "Such Gym does NOT exists!";

                return this.NotFound();
            }

            Gym gym = await this.gymService.GetGymByIdAsync(gymId);

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

            ApplicationUser user = await this.userService.GetUserByIdAsync(this.GetUserId());

            if (user.IsDeleted == true)
            {
                this.TempData[ErrorMessage] = "You were deleted by the Admin!";

                return this.RedirectToAction(nameof(Details), new { gymId = gymId });
            }

            try
            {
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
        public async Task<IActionResult> Joined([FromQuery] AllGymsQueryModel queryModel)
        {
            string userId = this.GetUserId();

            try
            {
                AllUserJoinedGymsFilteredAndPagedViewModel allGymsFilteredAndPagedViewModel = new AllUserJoinedGymsFilteredAndPagedViewModel
                {
                    UserId = userId,
                    TotalGymsCount = await this.gymService.GetAllUserJoinedGymsCountAsync(userId),
                    Gyms = await this.gymService.GetAllUserJoinedGymsAsync(userId, queryModel)
                };

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