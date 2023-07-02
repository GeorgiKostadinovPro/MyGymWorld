namespace MyGymWorld.Web.ViewModels.Users
{

    public class UserProfileViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string? Address { get; set; }

        public string? Role { get; set; }
    }
}
