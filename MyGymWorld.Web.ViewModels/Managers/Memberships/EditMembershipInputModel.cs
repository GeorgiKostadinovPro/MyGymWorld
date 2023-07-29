namespace MyGymWorld.Web.ViewModels.Managers.Memberships
{
	using MyGymWorld.Common;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class EditMembershipInputModel
	{
		public EditMembershipInputModel()
		{
			this.MembershipTypes = new HashSet<string>();
		}

		public string Id { get; set; } = null!;

		public string GymId { get; set; } = null!;

		[Range(typeof(decimal),
			ValidationalConstants.MembershipConstants.PriceMinValue,
			ValidationalConstants.MembershipConstants.PriceMaxValue)]
		[Display(Name = "Membership Price")]
		public decimal Price { get; set; }

		[Required]
		public string MembershipType { get; set; } = null!;

		public IEnumerable<string> MembershipTypes { get; set; }
	}
}
