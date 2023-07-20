namespace MyGymWorld.Web.ViewModels.Gyms
{
    using System.Collections.Generic;

    public class AllGymForDisplayViewModel
    {
        public AllGymForDisplayViewModel()
        {
            this.NewestGyms = new HashSet<DisplayGymViewModel>();
            this.MostLikedGyms = new HashSet<DisplayGymViewModel>();
        }
        
        public IEnumerable<DisplayGymViewModel> NewestGyms { get; set; } 
        
        public IEnumerable<DisplayGymViewModel> MostLikedGyms { get; set; }
    }
}
