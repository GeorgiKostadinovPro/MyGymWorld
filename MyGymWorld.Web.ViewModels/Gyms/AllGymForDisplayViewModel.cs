namespace MyGymWorld.Web.ViewModels.Gyms
{
    using System.Collections.Generic;

    public class AllGymForDisplayViewModel
    {
        public AllGymForDisplayViewModel()
        {
            this.Gyms = new List<DisplayGymViewModel>();
        }
        
        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public List<DisplayGymViewModel> Gyms { get; set; }
    }
}
