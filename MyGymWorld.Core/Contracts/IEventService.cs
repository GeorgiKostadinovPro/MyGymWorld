﻿namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Managers.Events;

    public interface IEventService
    {
        Task<Event> CreateEventAsync(CreateEventInputModel createEventInputModel);

        Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel);

        Task<int> GetAllActiveEventsCountByGymIdAsync(string gymId);

        IEnumerable<string> GetAllEventTypes();
    }
}
