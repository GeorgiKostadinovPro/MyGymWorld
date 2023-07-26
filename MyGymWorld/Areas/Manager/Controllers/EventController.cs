namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Events;
	using System.Globalization;
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
            try
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
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("AllForGym", "Event", new { area = "", GymId = gymId });
            }
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

                this.TempData[SuccessMessage] = "You created an event!";

                await this.notificationService.CreateNotificationAsync(
                    $"You created an event for {gym.Name}",
                    $"/Event/Details?eventId={createdEvent.Id.ToString()}",
                    userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("AllForGym", "Event", new { area = "", GymId = createEventInputModel.GymId });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string eventId, string gymId)
        {
            try
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

                bool doesEventExists = await this.eventService.CheckIfEventExistsByIdAsync(eventId);

                if (doesEventExists == false)
                {
                    this.TempData[ErrorMessage] = "Such event does NOT exist!";

                    return this.RedirectToAction("Details", "Event", new { area = "", eventId = eventId });
                }

                EditEventInputModel editEventInputModel = await this.eventService.GetEventForEditByIdAsync(eventId);

                editEventInputModel.EventTypes = this.eventService.GetAllEventTypes();

                return this.View(editEventInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Details", "Event", new { area = "", eventId = eventId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string eventId, EditEventInputModel editEventInputModel)
        {
            editEventInputModel.EventTypes = this.eventService.GetAllEventTypes();

            if (!this.ModelState.IsValid)
            {
                return this.View(editEventInputModel);
            }

            try
            {
                string userId = this.GetUserId();

                Gym gym = await this.gymService.GetGymByIdAsync(editEventInputModel.GymId);

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
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(editEventInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.Forbid();
                    }
                }

                var startDateResult = DateTime.TryParseExact(editEventInputModel.StartDate, "dd/MM/yyyy H:mm tt",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime startResult);

                var endDateResult = DateTime.TryParseExact(editEventInputModel.EndDate, "dd/MM/yyyy H:mm tt",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime endResult);

                if (startResult >= endResult)
                {
                    this.ModelState.AddModelError("StartDate", "The start date should be earlier than the end date!");

                    return this.View(editEventInputModel);
                }

                editEventInputModel.ParsedStartDate = startResult;
                editEventInputModel.ParsedEndDate = endResult;

                if (editEventInputModel.EventType == "None")
                {
                    this.ModelState.AddModelError("EventType", "You must choose an event type!");

                    return this.View(editEventInputModel);
                }

                editEventInputModel.Description = new HtmlSanitizer().Sanitize(editEventInputModel.Description);

                Event editedEvent = await this.eventService.EditEventAsync(eventId, editEventInputModel);

                this.TempData[SuccessMessage] = "You edited an event!";

                await this.notificationService.CreateNotificationAsync(
                    $"You edited an event for {gym.Name}",
                    $"/Event/Details?eventId={editedEvent.Id.ToString()}",
                    userId);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }

            return this.RedirectToAction("Details", "Event", new { area = "", eventId = eventId });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string eventId)
        {
            Event? eventToDelete = await this.eventService.GetEventByIdAsync(eventId);

            if (eventToDelete == null)
            {
                this.TempData[ErrorMessage] = "Such event does NOT exist!";

                return this.RedirectToAction("Details", "Event", new { area = "", eventId = eventId });
            }

            try
            {
                if (eventToDelete.StartDate < DateTime.UtcNow && eventToDelete.EndDate > DateTime.UtcNow)
                {
					this.TempData[ErrorMessage] = "You CANNOT delete running event!";

					return this.RedirectToAction("Details", "Event", new { area = "", eventId = eventId });
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

                await this.eventService.DeleteEventAsync(eventId);

                this.TempData[SuccessMessage] = "You deleted event!";

                return this.RedirectToActionPermanent("AllForGym", "Event", new { area = "", GymId = eventToDelete.GymId });
            }
            catch (Exception)
            {
				this.TempData[SuccessMessage] = "Something went wrong!";

                return this.RedirectToAction("Details", "Event", new { eventId = eventToDelete.Id });
            }
        }
    }
}