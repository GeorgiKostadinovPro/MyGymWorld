namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Managers.Events;

    public interface IEventService
    {
        Task<Event> CreateEventAsync(CreateEventInputModel createEventInputModel);

        Task<Event> EditEventAsync(string eventId, EditEventInputModel editEventInputModel);

        Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel);

        Task<int> GetAllActiveEventsCountByGymIdAsync(string gymId);

        Task<EventDetailsViewModel> GetEventDetailsByIdAsync(string eventId);

        Task<EditEventInputModel> GetEventForEditByIdAsync(string eventId);

        Task<bool> CheckIfEventExistsByIdAsync(string eventId);

        IEnumerable<string> GetAllEventTypes();
    }
}
