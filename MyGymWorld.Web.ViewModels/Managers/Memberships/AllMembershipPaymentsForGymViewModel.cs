namespace MyGymWorld.Web.ViewModels.Managers.Memberships
{
	using System.Collections.Generic;
	using MyGymWorld.Web.ViewModels.Memberships;

	public class AllMembershipPaymentsForGymViewModel
	{
        public AllMembershipPaymentsForGymViewModel()
        {
			this.Memberships = new List<PayedMembershipViewModel>();
        }

		public string GymId { get; set; } = null!;
		
		public int PagesCount { get; set; }

		public int CurrentPage { get; set; }

        public List<PayedMembershipViewModel> Memberships { get; set; }
	}
}
