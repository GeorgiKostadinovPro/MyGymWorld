namespace MyGymWorld.Web.ViewModels.Events
{
    public class AllUserJoinedEventsFilteredAndPagedViewModel : AllEventsForGymFilteredAndPagedViewModel
    {
        public string UserId { get; set; } = null!;
    }
}
