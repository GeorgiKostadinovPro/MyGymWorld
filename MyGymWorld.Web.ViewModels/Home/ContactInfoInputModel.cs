namespace MyGymWorld.Web.ViewModels.Home
{
    using System.ComponentModel.DataAnnotations;

    public class ContactInfoInputModel
    {
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;
    }
}
