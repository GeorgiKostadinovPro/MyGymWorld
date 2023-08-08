namespace MyGymWorld.Web.Areas.Manager.Controllers
{
    using Ganss.Xss;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Articles;
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
                string userId = this.GetUserId();

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

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
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
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

                return this.RedirectToAction("All", "Gym", new { area = "" });
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

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(createEventInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(createEventInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
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

                bool isEventTypeValid = Enum.TryParse<EventType>(createEventInputModel.EventType, true, out EventType result);

				if (createEventInputModel.EventType == "None" || isEventTypeValid == false)
                {
                    this.ModelState.AddModelError("EventType", "You must choose a valid event type!");

                    return this.View(createEventInputModel);
                }

                createEventInputModel.Description = new HtmlSanitizer().Sanitize(createEventInputModel.Description);

                Event createdEvent = await this.eventService.CreateEventAsync(createEventInputModel);

                this.TempData[SuccessMessage] = "You created an event!";

                await this.notificationService.CreateNotificationAsync(
                    $"You created an event for {gym.Name}",
                    $"/Event/Details?eventId={createdEvent.Id.ToString()}",
                userId);

                return this.RedirectToAction("AllForGym", "Event", new { area = "", gymId = createEventInputModel.GymId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string eventId, string gymId)
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

                Gym? gym = await this.gymService.GetGymByIdAsync(gymId);

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
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 });
                    }
                }

                bool doesEventExists = await this.eventService.CheckIfEventExistsByIdAsync(eventId);

                if (doesEventExists == false)
                {
                    this.TempData[ErrorMessage] = "Such event does NOT exist!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                EditEventInputModel editEventInputModel = await this.eventService.GetEventForEditByIdAsync(eventId);

                editEventInputModel.EventTypes = this.eventService.GetAllEventTypes();

                return this.View(editEventInputModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
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

                ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

                if (!this.User.IsInRole("Manager")
                    || user == null
                    || user.ManagerId == null)
                {
                    this.TempData[ErrorMessage] = "You are NOT a Manager!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                Gym? gym = await this.gymService.GetGymByIdAsync(editEventInputModel.GymId);

                if (gym == null)
                {
                    this.TempData[ErrorMessage] = "Such gym does NOT exists!";

                    return this.RedirectToAction("Index", "Home");
                }

                if (user.ManagerId != null)
                {
                    bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(editEventInputModel.GymId, user.ManagerId.ToString()!);

                    if (isGymManagedByCorrectManager == false)
                    {
                        return this.RedirectToAction("Error", "Home", new { statusCode = 403 }); ;
                    }
                }

                bool doesEventExists = await this.eventService.CheckIfEventExistsByIdAsync(eventId);

                if (doesEventExists == false)
                {
                    this.TempData[ErrorMessage] = "Such event does NOT exist!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                if (editEventInputModel.StartDate >= editEventInputModel.EndDate)
                {
                    this.ModelState.AddModelError("StartDate", "The start date should be earlier than the end date!");

                    return this.View(editEventInputModel);
                }

                bool isEventTypeValid = Enum.TryParse<EventType>(editEventInputModel.EventType, true, out EventType result);

                if (editEventInputModel.EventType == "None" || isEventTypeValid == false)
                {
                    this.ModelState.AddModelError("EventType", "You must choose a valid event type!");

                    return this.View(editEventInputModel);
                }

                editEventInputModel.Description = new HtmlSanitizer().Sanitize(editEventInputModel.Description);

                await this.eventService.EditEventAsync(eventId, editEventInputModel);

                this.TempData[SuccessMessage] = "You edited an event!";

                await this.notificationService.CreateNotificationAsync(
                    $"You edited an event for {gym.Name}",
                    $"/Event/Details?eventId={eventId}",
                    userId);

                return this.RedirectToAction("AllForGym", "Event", new { area = "", gymId = editEventInputModel.GymId });
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!"; 
                
                return this.RedirectToAction("All", "Gym", new { area = "" });
            }       
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string eventId)
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

                Event? eventToDelete = await this.eventService.GetEventByIdAsync(eventId);

                if (eventToDelete == null)
                {
                    this.TempData[ErrorMessage] = "Such event does NOT exist!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                if (eventToDelete.StartDate < DateTime.UtcNow && eventToDelete.EndDate > DateTime.UtcNow)
                {
                    this.TempData[ErrorMessage] = "You CANNOT delete running event!";

                    return this.RedirectToAction("All", "Gym", new { area = "" });
                }

                await this.eventService.DeleteEventAsync(eventId);

                this.TempData[SuccessMessage] = "You deleted event!";

                await this.notificationService.CreateNotificationAsync(
                 $"You deleted an event!",
                 $"/Event/AllForGym?gymId={eventToDelete.GymId.ToString()}",
                 userId);

                return this.RedirectToActionPermanent("AllForGym", "Event", new { area = "", GymId = eventToDelete.GymId });
            }
            catch (Exception)
            {
				this.TempData[SuccessMessage] = "Something went wrong!";

                return this.RedirectToAction("All", "Gym", new { area = "" });
            }
        }
    }
}