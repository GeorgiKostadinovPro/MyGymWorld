namespace MyGymWorld.Web.ViewModels.Administration.Roles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class RoleViewModel
    {
        public string Id { get; set; } = null!;


        public string Name { get; set; } = null!;

        public string CreatedOn { get; set; } = null!;

        public string? DeletedOn { get; set; }
    }
}
