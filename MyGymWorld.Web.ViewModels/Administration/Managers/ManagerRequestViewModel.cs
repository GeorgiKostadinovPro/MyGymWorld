namespace MyGymWorld.Web.ViewModels.Administration.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class ManagerRequestViewModel
    {
        public string ManagerId { get; set; } = null!;
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string ManagerProfilePictureUri { get; set; }

        public string ManagerType { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}
