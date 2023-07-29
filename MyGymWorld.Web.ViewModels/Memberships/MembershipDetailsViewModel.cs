namespace MyGymWorld.Web.ViewModels.Memberships
{
    public class MembershipDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string GymId { get; set; } = null!;

        public string GymName { get; set; } = null!;

        public string LogoUri { get; set; } = null!;

        public string Price { get; set; } = null!;

        public string MembershipType { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
