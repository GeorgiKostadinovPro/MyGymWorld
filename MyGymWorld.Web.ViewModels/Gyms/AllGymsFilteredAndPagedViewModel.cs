namespace MyGymWorld.Web.ViewModels.Gyms
{
    using System.Collections.Generic;

    public class AllGymsFilteredAndPagedViewModel
    {
        public AllGymsFilteredAndPagedViewModel()
        {
            this.Gyms = new HashSet<DisplayGymViewModel>();
        }

        public int TotalGymsCount { get; set; }

        public IEnumerable<DisplayGymViewModel> Gyms { get; set; }
    }
}
