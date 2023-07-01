namespace MyGymWorld.Web.ViewModels.Users
{
    using Microsoft.AspNetCore.Authentication;
    using System.ComponentModel.DataAnnotations;

    public class LoginUserInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
