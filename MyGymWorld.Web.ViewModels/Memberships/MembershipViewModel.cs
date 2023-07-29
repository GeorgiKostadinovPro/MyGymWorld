namespace MyGymWorld.Web.ViewModels.Memberships
{
    public class MembershipViewModel
    {
        public string Id { get; set; } = null!;

        public string GymId { get; set; } = null!;

        public string MembershipType { get; set; } = null!;

        public DateTime CreatedOn { get; set; }

        public string LogoUri { get; set; } = null!;
    }
}
