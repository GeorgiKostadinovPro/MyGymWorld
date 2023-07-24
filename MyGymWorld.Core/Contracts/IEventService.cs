namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Events;

    public interface IEventService
    {
        Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel);
    }
}
