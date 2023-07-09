namespace MyGymWorld.Web.ViewModels.Administration.Roles
{
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using System.Collections.Generic;

    public class AllRolesViewModel
    {
        public AllRolesViewModel()
        {
            this.Roles = new List<RoleViewModel>();
        }

        public List<RoleViewModel> Roles { get; set; }
    }
}
