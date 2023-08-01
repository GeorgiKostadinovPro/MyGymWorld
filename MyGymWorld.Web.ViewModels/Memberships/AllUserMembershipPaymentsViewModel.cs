namespace MyGymWorld.Web.ViewModels.Memberships
{
	using System.Collections.Generic;

	public class AllUserMembershipPaymentsViewModel
	{
        public AllUserMembershipPaymentsViewModel()
        {
			this.Memberships = new List<PayedMembershipViewModel>();
        }

        public string UserId { get; set; } = null!;

		public int PagesCount { get; set; }

		public int CurrentPage { get; set; }

		public List<PayedMembershipViewModel> Memberships { get; set; }
	}
}
