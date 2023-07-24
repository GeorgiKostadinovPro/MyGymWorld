namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel);

        Task<int> GetAllActiveEventsCountByGymIdAsync(string gymId);

        IEnumerable<string> GetAllEventTypes();
    }
}
