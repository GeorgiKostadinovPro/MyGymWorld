namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Events;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class EventController : ManagerController
    {
        private readonly IEventService eventService;
        private readonly IUserService userService;
        private readonly IGymService gymService;
        private readonly INotificationService notificationService;

        public EventController(
            IEventService _eventService,
            IUserService _userService,
            IGymService _gymService,
            INotificationService _notificationService)
        {
            this.eventService = _eventService;
            this.userService = _userService;
            this.gymService = _gymService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Create(string gymId)
        {
            Gym gym = await this.gymService.GetGymByIdAsync(gymId);

            if (gym == null)
            {
                this.TempData[ErrorMessage] = "You are NOT a Manager!";

                return this.RedirectToAction("Index", "Home");
            }

            string userId = this.GetUserId();

            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            if (!this.User.IsInRole("Manager")
                || user == null
                || user.ManagerId == null)
            {
                this.TempData[ErrorMessage] = "You are NOT a Manager!";

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

            CreateEventInputModel createEventInputModel = new CreateEventInputModel
            {
                GymId = gymId,
                EventTypes = this.eventService.GetAllEventTypes()
            };

            return this.View(createEventInputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEventInputModel createEventInputModel)
        {
            createEventInputModel.EventTypes = this.eventService.GetAllEventTypes();

            if (!this.ModelState.IsValid)
            {
                return this.View(createEventInputModel);
            }

            try
            {
                string userId = this.GetUserId();

                Gym gym = await this.gymService.GetGymByIdAsync(createEventInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("Index", "Home");
                }

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(createEventInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                if (createEventInputModel.StartDate >= createEventInputModel.EndDate)
                {
                    this.ModelState.AddModelError("StartDate", "The start date should be earlier than the end date!");

                    return this.View(createEventInputModel);
                }

                if (createEventInputModel.EventType == "None")
                {
                    this.ModelState.AddModelError("EventType", "You must choose an event type!");

                    return this.View(createEventInputModel);
                }

                createEventInputModel.Description = new HtmlSanitizer().Sanitize(createEventInputModel.Description);

                Event createdEvent = await this.eventService.CreateEventAsync(createEventInputModel);

                this.TempData[SuccessMessage] = "You created event!";

                await this.notificationService.CreateNotificationAsync(
                    $"You created an event for {gym.Name}",
                    $"/Event/Details?eventId={createdEvent.Id.ToString()}",
                    userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("AllForGym", "Event", new { area = "", GymId = createEventInputModel.GymId});
        }
    }
}
