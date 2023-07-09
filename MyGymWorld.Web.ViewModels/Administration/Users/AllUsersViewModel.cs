namespace MyGymWorld.Web.ViewModels.Administration.Users
{
    using System.Collections.Generic;

    public class AllUsersViewModel
    {
        public AllUsersViewModel()
        {
            this.Users = new List<UserViewModel>();
        }

        public List<UserViewModel> Users { get; set; }
    }
}
