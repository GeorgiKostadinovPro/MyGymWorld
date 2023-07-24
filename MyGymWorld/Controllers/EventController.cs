namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Events;

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

            return View(queryModel);
        }
    }
}
