namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Events;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class EventController : BaseController
    {
        private readonly IEventService eventService;

        public EventController(IEventService _eventService)
        {
            this.eventService = _eventService;
        }

        [HttpGet]
        public async Task<IActionResult> AllForGym([FromQuery] AllEventsForGymQueryModel queryModel)
        {
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

        [HttpGet]
        public async Task<IActionResult> Details(string eventId)
        {
            bool doesEventExist = await this.eventService.CheckIfEventExistsByIdAsync(eventId);

            if (doesEventExist == false)
            {
                this.TempData[ErrorMessage] = "Such event does NOT exist!";

                return this.NotFound();
            }

            EventDetailsViewModel eventDetailsViewModel = await this.eventService.GetEventDetailsByIdAsync(eventId);

            return this.View(eventDetailsViewModel);
        }
    }
}
