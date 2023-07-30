namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Memberships;
    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class MembershipController : BaseController
    {
        private readonly IMembershipService membershipService;
        private readonly IUserService userService;
        private readonly IGymService gymService;

        private readonly INotificationService notificationService;

        public MembershipController(
            IMembershipService _membershipService,
            IUserService _userService,
            IGymService _gymService,
            INotificationService _notificationService)
        {
            this.membershipService = _membershipService;
                
            this.userService = _userService;
            this.gymService = _gymService;

            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> AllforGym([FromQuery] AllMembershipsForGymQueryModel queryModel)
        {
            Gym? gym = await this.gymService.GetGymByIdAsync(queryModel.GymId);

            if (gym == null)
            {
                this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                return this.RedirectToAction("Index", "Home");
            }

            string userId = this.GetUserId();

            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            if (user.ManagerId == null
                || (user.ManagerId != null && !(await this.gymService.CheckIfGymIsManagedByManagerAsync(queryModel.GymId, user.ManagerId.ToString()!))))
            {
                bool hasUserJoinedGym = await this.gymService.CheckIfGymIsJoinedByUserAsync(queryModel.GymId, userId);

                if (hasUserJoinedGym == false)
                {
                    this.TempData[ErrorMessage] = "You have to JOIN the gym to see events!";

                    return this.RedirectToAction("Details", "Gym", new { gymId = queryModel.GymId });
                }
            }

            AllMembershipsForGymFilteredAndPagedViewModel allMembershipsForGymFilteredAndPagedViewModel = new AllMembershipsForGymFilteredAndPagedViewModel
            {
                TotalMembershipsCount = await this.membershipService.GetAllActiveMembershipsCountByGymIdAsync(queryModel.GymId),
                Memberships = await this.membershipService.GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(queryModel)
            };

            queryModel.MembershipTypes = this.membershipService.GetAllMembershipTypes();
            queryModel.TotalMembershipsCount = allMembershipsForGymFilteredAndPagedViewModel.TotalMembershipsCount;
            queryModel.Memberships = allMembershipsForGymFilteredAndPagedViewModel.Memberships;

            return this.View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string membershipId)
        {
            Membership? membershipToDisplay = await this.membershipService.GetMembershipByIdAsync(membershipId);

            if (membershipToDisplay == null)
            {
                this.TempData[ErrorMessage] = "Such membership does NOT exist!";

                return this.RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            MembershipDetailsViewModel membershipDetailsViewModel = await this.membershipService.GetMembershipDetailsByIdAsync(membershipId);

            return this.View(membershipDetailsViewModel);
        }

        public async Task<IActionResult> Buy(string membershipId)
        {
            Membership? membershipToBuy = await this.membershipService.GetMembershipByIdAsync(membershipId);

            if (membershipToBuy == null)
            {
                this.TempData[ErrorMessage] = "Such membership does NOT exist!";

                return this.RedirectToAction("Error", "Home", new { statusCode = 404 });
            }

            try
            {
                Gym? gym = await this.gymService.GetGymByIdAsync(membershipToBuy.GymId.ToString());

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (user.ManagerId == null
                    || (user.ManagerId != null && !(await this.gymService.CheckIfGymIsManagedByManagerAsync(membershipToBuy.GymId.ToString(), user.ManagerId.ToString()!))))
                {
                    bool hasUserJoinedGym = await this.gymService.CheckIfGymIsJoinedByUserAsync(membershipToBuy.GymId.ToString(), userId);

                    if (hasUserJoinedGym == false)
                    {
                        this.TempData[ErrorMessage] = "You have to JOIN the gym to buy a membership!";

                        return this.RedirectToAction("Details", "Gym", new { gymId = membershipToBuy.GymId });
                    }
                }


                await this.membershipService.BuyMembershipAsync(membershipId, userId);

                this.TempData[SuccessMessage] = "You successfully bought a membership!";

                await this.notificationService.CreateNotificationAsync(
                    $"You bought a membership for {gym.Name}",
                    $"/Membership/UserMemberships?userId={userId}",
                    userId);

                return this.RedirectToAction(nameof(UserMemberships), new { UserId = userId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
                
                return this.RedirectToAction(nameof(Details), new { membershipId = membershipId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserMemberships([FromQuery] AllUserMemberhipsQueryModel queryModel)
        {
            try
            {
                string userId = this.GetUserId();

                AllUserMembershipsFilteredAndPagedViewModel allUserMembershipsFilteredAndPagedViewModel = new AllUserMembershipsFilteredAndPagedViewModel
                {
                    UserId = userId,
                    TotalMembershipsCount = await this.membershipService.GetAllUserMembershipsCountByUserIdAsync(userId),
                    Memberships = await this.membershipService.GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsync(userId, queryModel)
                };

                queryModel.MembershipTypes = this.membershipService.GetAllMembershipTypes();
                queryModel.TotalMembershipsCount = allUserMembershipsFilteredAndPagedViewModel.TotalMembershipsCount;
                queryModel.Memberships = allUserMembershipsFilteredAndPagedViewModel.Memberships;

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