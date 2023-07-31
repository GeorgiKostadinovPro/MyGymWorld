namespace MyGymWorld.Web.ViewModels.Accounts
{
    using System.ComponentModel.DataAnnotations;

    public class EnterEmailInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
