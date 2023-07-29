namespace MyGymWorld.Web.ViewModels.Memberships
{
    using System.ComponentModel.DataAnnotations;
    using MyGymWorld.Web.ViewModels.Memberships.Enums; 
    
    using static MyGymWorld.Common.GlobalConstants;

    public class AllMembershipsForGymQueryModel
    {
        public AllMembershipsForGymQueryModel()
        {
            this.MembershipsPerPage = MembershipConstants.MembershipsPerPage;
            this.CurrentPage = MembershipConstants.DefaultPage;
            this.MembershipTypes = new HashSet<string>();

            this.Memberships = new HashSet<MembershipViewModel>();
        }

        public string GymId { get; set; } = null!;

        public int MembershipsPerPage { get; set; }

        public string? MembershipType { get; set; }

        [Display(Name = "Search by text")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Sort Memberships By")]
        public MembershipsSorting MembershipsSorting { get; set; }

        public int CurrentPage { get; set; }

        public int TotalMembershipsCount { get; set; }

        public IEnumerable<string> MembershipTypes { get; set; }

        public IEnumerable<MembershipViewModel> Memberships { get; set; }
    }
}
