namespace MyGymWorld.Web.ViewModels.Managers.Gyms
{
    using System.Collections.Generic;

    public class AllGymsForManagementViewModel
    {
        public AllGymsForManagementViewModel()
        {
            this.Gyms = new List<GymViewModel>();
        }

        public int PagesCount { get; set; }

        public int CurrentPage { get; set; }

        public List<GymViewModel> Gyms { get; set; }
    }
}
