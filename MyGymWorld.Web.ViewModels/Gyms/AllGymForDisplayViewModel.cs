namespace MyGymWorld.Web.ViewModels.Gyms
{
    using System.Collections.Generic;

    public class AllGymForDisplayViewModel
    {
        public AllGymForDisplayViewModel()
        {
            this.GymTypes = new HashSet<string>();

            this.NewestGyms = new HashSet<DisplayGymViewModel>();
            this.MostLikedGyms = new HashSet<DisplayGymViewModel>();

            this.AllGyms = new HashSet<DisplayGymViewModel>();
        }
        
        public int PagesCount { get; set; }

        public int CurrentPage { get; set; } 
        
        public int TotalGyms { get; set; }

        public IEnumerable<string> GymTypes { get; set; }

        public IEnumerable<DisplayGymViewModel> NewestGyms { get; set; } 
        
        public IEnumerable<DisplayGymViewModel> MostLikedGyms { get; set; }

        public IEnumerable<DisplayGymViewModel> AllGyms { get; set; }
    }
}
