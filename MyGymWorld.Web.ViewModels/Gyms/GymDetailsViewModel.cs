namespace MyGymWorld.Web.ViewModels.Gyms
{
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Models;

    public class GymDetailsViewModel
    {
        public GymDetailsViewModel()
        {
            this.GymImages = new HashSet<string>();
        }

        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string LogoUri { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!;

        public string GymType { get; set; } = null!;

        public string ManagerId { get; set; } = null!;

        public string ManagerFullName { get; set; } = null!;

        public string Address { get; set; } = null!;

        public IEnumerable<string> GymImages { get; set; }
    }
}
