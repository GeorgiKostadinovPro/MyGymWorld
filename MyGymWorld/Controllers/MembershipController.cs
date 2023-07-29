namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Memberships;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class MembershipController : BaseController
    {
        private readonly IMembershipService membershipService;
        private readonly IUserService userService;
        private readonly IGymService gymService;

        public MembershipController(
            IMembershipService _membershipService,
            IUserService _userService,
            IGymService _gymService)
        {
            this.membershipService = _membershipService;
                
            this.userService = _userService;
            this.gymService = _gymService;
        }

        [HttpGet]
        public async Task<IActionResult> AllforGym([FromQuery] AllMembershipsForGymQueryModel queryModel)
        {
            Gym gym = await this.gymService.GetGymByIdAsync(queryModel.GymId);

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

                return this.NotFound();
            }

            MembershipDetailsViewModel membershipDetailsViewModel = await this.membershipService.GetMembershipDetailsByIdAsync(membershipId);

            return this.View(membershipDetailsViewModel);
        }
    }
}
