namespace MyGymWorld.Web.ViewModels.Administration.Users
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserViewModel
    {
        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? ProfilePictureUri { get; set; }

        public string Role { get; set; } = null!;

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public string CreatedOn { get; set; } = null!;
    }
}
