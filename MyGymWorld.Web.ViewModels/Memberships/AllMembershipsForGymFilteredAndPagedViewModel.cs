namespace MyGymWorld.Web.ViewModels.Memberships
{
    using System.Collections.Generic;

    public class AllMembershipsForGymFilteredAndPagedViewModel
    {
        public AllMembershipsForGymFilteredAndPagedViewModel()
        {
            this.Memberships = new HashSet<MembershipViewModel>();

        }
        public int TotalMembershipsCount { get; set; }

        public IEnumerable<MembershipViewModel> Memberships { get; set; }
    }
}
