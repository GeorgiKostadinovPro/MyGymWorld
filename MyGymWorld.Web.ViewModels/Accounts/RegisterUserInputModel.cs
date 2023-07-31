namespace MyGymWorld.Web.ViewModels.Accounts
{
    using MyGymWorld.Common;
    using System.ComponentModel.DataAnnotations;

    public class RegisterUserInputModel
    {
        [Required]
        [StringLength(ApplicationUserConstants.UsernameMaxLength, ErrorMessage = "The username must be at least 5 and at max 50 characters long.",
            MinimumLength = ApplicationUserConstants.UsernameMinLength)]
        public string UserName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "The password must be at least 6 and at max 100 characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        public string? ReturnUrl { get; set; }
    }
}
