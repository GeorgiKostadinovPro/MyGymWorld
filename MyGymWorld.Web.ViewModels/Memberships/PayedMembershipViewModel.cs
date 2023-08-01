namespace MyGymWorld.Web.ViewModels.Memberships
{
	using System;

	public class PayedMembershipViewModel
	{
		public string Id { get; set; } = null!;

		public string UserEmail { get; set; } = null!;
		
		public DateTime PurchasedOn { get; set; }

		public DateTime ValidTo { get; set; }

		public string MembershipType { get; set; } = null!;

		public bool PaymentStatus { get; set; }
	}
}
