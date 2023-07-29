namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Events;
    using MyGymWorld.Web.ViewModels.Managers.Memberships;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class MembershipController : ManagerController
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
        public async Task<IActionResult> Create(string gymId)
        {
            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Gym gym = await this.gymService.GetGymByIdAsync(gymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                CreateMembershipInputModel createMembershipInputModel = new CreateMembershipInputModel
                {
                    GymId = gymId,
                    MembershipTypes = this.membershipService.GetAllMembershipTypes()
                };

                return this.View(createMembershipInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("AllForGym", "Article", new { area = "", GymId = gymId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateMembershipInputModel createMembershipInputModel)
        {
            createMembershipInputModel.MembershipTypes = this.membershipService.GetAllMembershipTypes();

            if (!this.ModelState.IsValid)
            {
                return this.View(createMembershipInputModel);
            }

            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Gym gym = await this.gymService.GetGymByIdAsync(createMembershipInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(createMembershipInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                bool isMembershipTypeValid = Enum.TryParse<MembershipType>(createMembershipInputModel.MembershipType, true, out MembershipType result);

                if (createMembershipInputModel.MembershipType == "None" || isMembershipTypeValid == false)
                {
                    this.ModelState.AddModelError("MembershipType", "You must choose a valid memebrship type!");

                    return this.View(createMembershipInputModel);
                }

                Membership createdMembership = await this.membershipService.CreateMembershipAsync(createMembershipInputModel);

                this.TempData[SuccessMessage] = "You created a membership!";

                await this.notificationService.CreateNotificationAsync(
                    $"You created a membership for {gym.Name}",
                    $"/Membership/Details?membershipId={createdMembership.Id.ToString()}",
                    userId);

                return this.RedirectToAction("AllForGym", "Membership", new { area = "", gymId = createMembershipInputModel.GymId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }

		[HttpGet]
		public async Task<IActionResult> Edit(string membershipId, string gymId)
		{
			try
			{
				string userId = this.GetUserId();

				ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

				if (!this.User.IsInRole("Manager")
					|| user == null
					|| user.ManagerId == null)
				{
					this.TempData[ErrorMessage] = "You are NOT a Manager!";

					return this.RedirectToAction("All", "Gym", new { area = "" });
				}

				Gym gym = await this.gymService.GetGymByIdAsync(gymId);

				if (gym == null)
				{
					this.TempData[ErrorMessage] = "Such gym does NOT exists!";

					return this.RedirectToAction("Index", "Home");
				}

				if (user.ManagerId != null)
				{
					bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(gymId, user.ManagerId.ToString()!);

					if (isGymManagedByCorrectManager == false)
					{
						return this.Forbid();
					}
				}

				bool doesEventExists = await this.membershipService.CheckIfMembershipExistsByIdAsync(membershipId);

				if (doesEventExists == false)
				{
					this.TempData[ErrorMessage] = "Such membership does NOT exist!";

					return this.RedirectToAction("All", "Gym", new { area = "" });
				}

				EditMembershipInputModel editMembershipInputModel = await this.membershipService.GetMembershipForEditByIdAsync(membershipId);

				editMembershipInputModel.MembershipTypes = this.membershipService.GetAllMembershipTypes();

				return this.View(editMembershipInputModel);
			}
			catch (Exception)
			{
				this.TempData[ErrorMessage] = "Something went wrong!";

				return this.RedirectToAction("All", "Gym", new { area = "" });
			}
		}

        [HttpPost]
        public async Task<IActionResult> Edit(string membershipId, EditMembershipInputModel editMembershipInputModel)
        {
            editMembershipInputModel.MembershipTypes = this.membershipService.GetAllMembershipTypes();

            if (!this.ModelState.IsValid)
            {
                return this.View(editMembershipInputModel);
            }

            try
            {
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Gym gym = await this.gymService.GetGymByIdAsync(editMembershipInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(editMembershipInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                bool isMembershipTypeValid = Enum.TryParse<MembershipType>(editMembershipInputModel.MembershipType, true, out MembershipType result);

                if (editMembershipInputModel.MembershipType == "None" || isMembershipTypeValid == false)
                {
                    this.ModelState.AddModelError("MembershipType", "You must choose a valid membership type!");

                    return this.View(editMembershipInputModel);
                }

                Membership editedMembership = await this.membershipService.EditMembershipAsync(membershipId, editMembershipInputModel);

                this.TempData[SuccessMessage] = "You edited a membership!";

                await this.notificationService.CreateNotificationAsync(
                    $"You edited a membership for {gym.Name}",
                    $"/Membership/Details?membershipId={editedMembership.Id.ToString()}",
                    userId);

                return this.RedirectToAction("AllForGym", "Membership", new { area = "", gymId = editMembershipInputModel.GymId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }
    }
}