﻿namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
	using MyGymWorld.Core.Contracts;
	using MyGymWorld.Data.Models;
	using MyGymWorld.Web.ViewModels.Events;
    using System;
	using static MyGymWorld.Common.NotificationMessagesConstants;

    public class EventController : BaseController
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
        public async Task<IActionResult> AllForGym([FromQuery] AllEventsForGymQueryModel queryModel)
        {
            try
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

                AllEventsForGymFilteredAndPagedViewModel allEventsForGymFilteredAndPagedViewModel = new AllEventsForGymFilteredAndPagedViewModel()
                {
                    TotalEventsCount = await this.eventService.GetAllActiveEventsCountByGymIdAsync(queryModel.GymId),
                    Events = await this.eventService.GetAllActiveEventsFilteredAndPagedByGymIdAsync(queryModel)
                };

                queryModel.EventTypes = this.eventService.GetAllEventTypes();
                queryModel.TotalEventsCount = allEventsForGymFilteredAndPagedViewModel.TotalEventsCount;
                queryModel.Events = allEventsForGymFilteredAndPagedViewModel.Events;

                return this.View(queryModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string eventId)
        {
            try
            {
                Event? eventToDisplay = await this.eventService.GetEventByIdAsync(eventId);

                if (eventToDisplay == null)
                {
                    this.TempData[ErrorMessage] = "Such event does NOT exist!";

                    return this.RedirectToAction("Error", "Home", new { area = "", statusCode = 404 });
                }

                EventDetailsViewModel eventDetailsViewModel = await this.eventService.GetEventDetailsByIdAsync(eventId);
                
                return this.View(eventDetailsViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Participate(string eventId)
        {
            try
            {
                Event? eventToParticipate = await this.eventService.GetEventByIdAsync(eventId);

                if (eventToParticipate == null)
                {
                    this.TempData[ErrorMessage] = "You tried joining non-existing event!";

                    return this.NotFound();
                }

				string userId = this.GetUserId();

				ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

				if (user.ManagerId != null)
				{
					bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(eventToParticipate.GymId.ToString(), user.ManagerId.ToString()!);

					if (isGymManagedByCorrectManager == true)
					{
						return this.Forbid();
					}
				}

                if (eventToParticipate.EndDate <= DateTime.UtcNow)
                {
					this.TempData[ErrorMessage] = "This event has already ended!";

					return this.RedirectToAction(nameof(Details), new { eventId = eventId });
				}

				bool hasUserAlreadyJoinedEvent = await this.eventService.CheckIfUserHasAlreadyJoinedEventAsync(eventId, userId);

                if (hasUserAlreadyJoinedEvent)
                {
                    this.TempData[ErrorMessage] = "You are already a participant in this event!";

                    return this.RedirectToAction(nameof(Details), new { eventId = eventId });
                }

                await this.eventService.ParticipateInEventAsync(eventId, userId);

                await this.notificationService.CreateNotificationAsync(
                    $"You are now participating in {eventToParticipate.Name}",
                    $"/Event/Details?eventId={eventId}",
                    userId);

                return this.RedirectToAction(nameof(Joined), new { UserId = userId });
			}
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction(nameof(Details), new { eventId = eventId });
            } 
        }

		[HttpPost]
		public async Task<IActionResult> Leave(string eventId)
		{
			try
			{
				Event? eventToLeave = await this.eventService.GetEventByIdAsync(eventId);

				if (eventToLeave == null)
				{
					this.TempData[ErrorMessage] = "You tried leaving non-existing event!";

					return this.NotFound();
				}

				string userId = this.GetUserId();

				ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

				if (user.ManagerId != null)
				{
					bool isGymManagedByCorrectManager = await this.gymService.CheckIfGymIsManagedByManagerAsync(eventToLeave.GymId.ToString(), user.ManagerId.ToString()!);

					if (isGymManagedByCorrectManager == true)
					{
						return this.Forbid();
					}
				}

				if (eventToLeave.EndDate <= DateTime.UtcNow)
				{
					this.TempData[ErrorMessage] = "This event has already ended!";

					return this.RedirectToAction(nameof(Details), new { eventId = eventId });
				}

				bool hasUserAlreadyLeftEvent = await this.eventService.CheckIfUserHasAlreadyLeftEventAsync(eventId, userId);

				if (hasUserAlreadyLeftEvent)
				{
					this.TempData[ErrorMessage] = "You have already left this event!";

					return this.RedirectToAction(nameof(Details), new { eventId = eventId });
				}

				await this.eventService.LeaveEventAsync(eventId, userId);

				await this.notificationService.CreateNotificationAsync(
					$"You left the event: {eventToLeave.Name}",
					$"/Event/Details?eventId={eventId}",
					userId);

				return this.RedirectToAction(nameof(Joined), new { UserId = userId });
			}
			catch (Exception)
			{
				this.TempData[ErrorMessage] = "Something went wrong!";

				return this.RedirectToAction(nameof(Details), new { eventId = eventId });
			}
		}

		[HttpGet]
        public async Task<IActionResult> Joined([FromQuery] AllUserJoinedEventsQueryModel queryModel)
        {
            try
            {
                string userId = this.GetUserId();

                AllUserJoinedEventsFilteredAndPagedViewModel allGymsFilteredAndPagedViewModel = new AllUserJoinedEventsFilteredAndPagedViewModel
                {
                    UserId = userId,
                    TotalEventsCount = await this.eventService.GetAllUserJoinedEventsCountAsync(userId),
                    Events = await this.eventService.GetAllUserJoinedEventsFilteredAndPagedAsync(userId, queryModel)
                };

                queryModel.EventTypes = this.eventService.GetAllEventTypes();
                queryModel.TotalEventsCount = allGymsFilteredAndPagedViewModel.TotalEventsCount;
                queryModel.Events = allGymsFilteredAndPagedViewModel.Events;

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