namespace MyGymWorld.Web.ViewModels.Gyms
{
    using System.Collections.Generic;

    public class AllGymForDisplayViewModel
    {
        public AllGymForDisplayViewModel()
        {
            this.Gyms = new HashSet<DisplayGymViewModel>();
        }
        
        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<DisplayGymViewModel> Gyms { get; set; }
    }
}
