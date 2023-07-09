namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Administration.Roles;

    using static Common.NotificationMessagesConstants;

    public class RoleController : AdminController
    {
        private readonly IRoleService roleService;
        private readonly INotificationService notificationService;

        public RoleController(IRoleService _roleService, INotificationService _notificationService)
        {
            this.roleService = _roleService;
            this.notificationService = _notificationService;
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

        [HttpGet]
        public IActionResult Create()
        {
            CreateRoleInputModel createRoleInputModel = new CreateRoleInputModel();

            return this.View(createRoleInputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRoleInputModel createRoleInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(createRoleInputModel);
            }

            try
            {
                bool doesRoleExists = await this.roleService.CheckIfRoleAlreadyExistsByNameAsync(createRoleInputModel.Name);

                if (doesRoleExists)
                {
                    throw new InvalidOperationException(ExceptionConstants.ApplicationRoleErrors.RoleAlreadyExists);
                }

                await this.roleService.CreateRoleAsync(createRoleInputModel);

                await this.notificationService.CreateNotificationAsync(
                    $"You successfully created role: {createRoleInputModel.Name}",
                    "/Admin/Role/Active",
                    this.GetUserId());
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);

                return this.View(createRoleInputModel);
            }

            return this.RedirectToAction(nameof(Active));
        }

        public async Task<IActionResult> Delete(string roleId)
        {
            try
            {
                bool doesRoleExists = await this.roleService.CheckIfRoleAlreadyExistsByIdAsync(roleId);

                if (!doesRoleExists)
                {
                    throw new InvalidOperationException(ExceptionConstants.ApplicationRoleErrors.RoleAlreadyExists);
                }

                ApplicationRole role = await this.roleService.DeleteRoleAsync(roleId);

                await this.notificationService.CreateNotificationAsync(
                    $"You successfully deleted role: {role.Name}",
                    "/Admin/Role/Deleted",
                    this.GetUserId());
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction(nameof(Active));
            }

            return this.RedirectToAction(nameof(Deleted));
        }
    }
}
