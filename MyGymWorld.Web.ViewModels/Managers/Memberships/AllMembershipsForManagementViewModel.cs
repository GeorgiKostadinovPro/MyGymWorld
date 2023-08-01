namespace MyGymWorld.Web.ViewModels.Managers.Memberships
{
	using System.Collections.Generic;

	public class AllPurchasedMembershipsForGymViewModel
	{
        public AllPurchasedMembershipsForGymViewModel()
        {
			this.Memberships = new List<PurchasedMembershipViewModel>();
        }

		public string GymId { get; set; } = null!;
		
		public int PagesCount { get; set; }

		public int CurrentPage { get; set; }

        public List<PurchasedMembershipViewModel> Memberships { get; set; }
	}
}
