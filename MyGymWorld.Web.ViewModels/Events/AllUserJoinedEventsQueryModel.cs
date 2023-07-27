namespace MyGymWorld.Web.ViewModels.Events
{
    using MyGymWorld.Web.ViewModels.Events.Enums;
    using System.ComponentModel.DataAnnotations;
    using static MyGymWorld.Common.GlobalConstants;

    public class AllUserJoinedEventsQueryModel
    {
        public AllUserJoinedEventsQueryModel()
        {
            this.EventsPerPage = EventConstants.EventsPerPage;
            this.CurrentPage = EventConstants.DefaultPage;
            this.EventTypes = new HashSet<string>();

            this.Events = new HashSet<EventViewModel>();
        }

        public string UserId { get; set; } = null!;

        public int EventsPerPage { get; set; }

        public string? EventType { get; set; } = null!;

        [Display(Name = "Search by text")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Sort Events By")]
        public EventsSorting EventsSorting { get; set; }

        public int CurrentPage { get; set; }

        public int TotalEventsCount { get; set; }

        public IEnumerable<string> EventTypes { get; set; }

        public IEnumerable<EventViewModel> Events { get; set; }
    }
}
