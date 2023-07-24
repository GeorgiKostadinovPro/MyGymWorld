namespace MyGymWorld.Web.ViewModels.Events
{
    using System.Collections.Generic;

    public class AllEventsForGymFilteredAndPagedViewModel
    {
        public AllEventsForGymFilteredAndPagedViewModel()
        {
            this.Events = new HashSet<EventViewModel>();
        }

        public int TotalGymsCount { get; set; }

        public IEnumerable<EventViewModel> Events { get; set; }
    }
}
