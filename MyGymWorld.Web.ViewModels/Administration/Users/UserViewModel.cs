namespace MyGymWorld.Web.ViewModels.Administration.Users
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;

        public string? ManagerId { get; set; }

        public string Email { get; set; } = null!;

        public string? ProfilePictureUri { get; set; }

        public string Role { get; set; } = null!;

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public string CreatedOn { get; set; } = null!;

        public string? DeletedOn { get; set; }
    }
}
