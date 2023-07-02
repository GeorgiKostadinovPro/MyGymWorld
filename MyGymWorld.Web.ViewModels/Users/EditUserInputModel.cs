namespace MyGymWorld.Web.ViewModels.Users
{
    using System.ComponentModel.DataAnnotations;

    public class EditUserInputModel
    {
        public string Id { get; set; } = null!;

        [Required]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ProfilePictureUrl { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }
    }
}
