namespace MyGymWorld.Web.ViewModels.Gyms
{
    using MyGymWorld.Web.ViewModels.Gyms.Enums;
    using System.ComponentModel.DataAnnotations;

    using static MyGymWorld.Common.GlobalConstants;

    public class AllGymsQueryModel
    {
        public AllGymsQueryModel()
        {
            this.GymsPerPage = GymConstants.GymsPerPage;
            this.CurrentPage = GymConstants.DefaultPage;
            this.GymTypes = new HashSet<string>();

            this.Gyms = new HashSet<DisplayGymViewModel>();
        }

        public int GymsPerPage { get; set; }

        public string? GymType { get; set; }

        [Display(Name = "Search by text")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Sort Gyms By")]
        public GymsSorting Sorting { get; set; }

        public int CurrentPage { get; set; }

        public int TotalGymsCount { get; set; }

        public IEnumerable<string> GymTypes { get; set; }

        public IEnumerable<DisplayGymViewModel> Gyms { get; set; }
    }
}
