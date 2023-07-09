namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Administration.Roles;

    public class RoleController : AdminController
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService _roleService)
        {
            this.roleService = _roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Active()
        {
            AllRolesViewModel allRolesViewModel = new AllRolesViewModel
            {
                Roles = await this.roleService.GetActiveForAdministrationAsync()
            };

            return View(allRolesViewModel);       
        }

        [HttpGet]
        public async Task<IActionResult> Deleted()
        {
            AllRolesViewModel allRolesViewModel = new AllRolesViewModel
            {
                Roles = await this.roleService.GetDeletedForAdministrationAsync()
            };

            return View(allRolesViewModel);
        }
    }
}
