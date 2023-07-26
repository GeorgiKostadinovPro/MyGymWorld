namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Managers.Events;

    public interface IEventService
    {
        Task<Event> CreateEventAsync(CreateEventInputModel createEventInputModel);

        Task<Event> EditEventAsync(string eventId, EditEventInputModel editEventInputModel);

        Task DeleteEventAsync(string eventId);

        Task ParticipateInEventAsync(string eventId, string userId);

        Task LeaveEventAsync(string eventId, string userId);

		Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel);

        Task<int> GetAllActiveEventsCountByGymIdAsync(string gymId);

        Task<IEnumerable<EventViewModel>> GetAllUserJoinedEventsFilteredAndPagedAsync(string userId, AllUserJoinedEventsQueryModel queryModel);

        Task<int> GetAllUserJoinedEventsCountAsync(string userId);

        Task<EventDetailsViewModel> GetEventDetailsByIdAsync(string eventId);

        Task<EditEventInputModel> GetEventForEditByIdAsync(string eventId);

        Task<Event?> GetEventByIdAsync(string eventId);
        
        Task<bool> CheckIfEventExistsByIdAsync(string eventId);

		Task<bool> CheckIfUserHasAlreadyJoinedEventByIdAsync(string eventId, string userId);

        Task<bool> CheckIfUserHasAlreadyLeftEventByIdAsync(string eventId, string userId);

        Task<int> GetAllActiveEventsCountAsync();


		IEnumerable<string> GetAllEventTypes();
    }
}
