namespace MyGymWorld.Web.ViewModels.Managers.Gyms
{
    public class CreateGymInputModel
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string LogoUrl { get; set; } = null!;

        public string WebsiteUrl { get; set; } = null!;
    }
}
