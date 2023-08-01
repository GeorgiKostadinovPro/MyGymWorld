namespace MyGymWorld.Web.ViewModels.Managers.Memberships
{
	using System;

	public class PurchasedMembershipViewModel
	{
		public string Id { get; set; } = null!;

		public string UserEmail { get; set; } = null!;
		
		public DateTime PurchasedOn { get; set; }

		public DateTime ValidTo { get; set; }

		public string MembershipType { get; set; } = null!;

		public bool PaymentStatus { get; set; }
	}
}
