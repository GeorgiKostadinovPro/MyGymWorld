namespace MyGymWorld.Web.ViewModels.Administration.Managers
{
    using System.Collections.Generic;

    public class AllRequestsViewModel
    {
        public AllRequestsViewModel()
        {
            this.Requests = new HashSet<ManagerRequestViewModel>();
        }

        public string AdminId { get; set; }

        public IEnumerable<ManagerRequestViewModel> Requests { get; set; }
    }
}
