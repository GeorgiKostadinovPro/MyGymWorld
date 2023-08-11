namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Administration.Roles;

    using static Common.NotificationMessagesConstants;

    public class RoleController : AdminController
    {
        private const int RolesPerPage = 2;

        private readonly IRoleService roleService;
        private readonly INotificationService notificationService;

        public RoleController(IRoleService _roleService, INotificationService _notificationService)
        {
            this.roleService = _roleService;
            this.notificationService = _notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Active(int page = 1)
        {
            try
            {
                int rolesCount = await this.roleService.GetActiveOrDeletedRolesCountAsync(false);

                int totalPages = (int)Math.Ceiling((double)(rolesCount / RolesPerPage));
                totalPages = totalPages == 0 ? 1 : totalPages;

                AllRolesViewModel allRolesViewModel = new AllRolesViewModel
                {
                    Roles = await this.roleService.GetActiveOrDeletedForAdministrationAsync(false, (page - 1) * RolesPerPage, RolesPerPage),
                    CurrentPage = page,
                    PagesCount = totalPages
                };

                return View(allRolesViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Dashboard", "Manager", new { area = "Admin" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Deleted(int page = 1)
        {
            try
            {
                int rolesCount = await this.roleService.GetActiveOrDeletedRolesCountAsync(true);

                int totalPages = (int)Math.Ceiling((double)(rolesCount / RolesPerPage));

                totalPages = totalPages == 0 ? 1 : totalPages;

                AllRolesViewModel allRolesViewModel = new AllRolesViewModel
                {
                    Roles = await this.roleService.GetActiveOrDeletedForAdministrationAsync(true, (page - 1) * RolesPerPage, RolesPerPage),
                    CurrentPage = page,
                    PagesCount = totalPages
                };

                return View(allRolesViewModel);
            }
            catch (Exception)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";

                return this.RedirectToAction("Dashboard", "Manager", new { area = "Admin" });
            }
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
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(createRoleInputModel);
                }

                bool doesRoleExists = await this.roleService.CheckIfRoleAlreadyExistsByNameAsync(createRoleInputModel.Name);

                if (doesRoleExists)
                {
                    throw new InvalidOperationException(ExceptionConstants.ApplicationRoleErrors.RoleAlreadyExists);
                }

                await this.roleService.CreateRoleAsync(createRoleInputModel);

                this.TempData[SuccessMessage] = "You successfully created a role!";

                await this.notificationService.CreateNotificationAsync(
                    $"You successfully created role: {createRoleInputModel.Name}",
                    "/Admin/Role/Active",
                    this.GetUserId());
                
                return this.RedirectToAction(nameof(Active));
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);

                return this.View(createRoleInputModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string roleId)
        {
            try
            {
                bool doesRoleExists = await this.roleService.CheckIfRoleAlreadyExistsByIdAsync(roleId);

                if (!doesRoleExists)
                {
                    throw new InvalidOperationException(ExceptionConstants.ApplicationRoleErrors.InvalidRoleId);
                }

                EditRoleInputModel editRoleInputModel = await this.roleService.GetRoleForEditAsync(roleId);

                return this.View(editRoleInputModel);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction(nameof(Active));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditRoleInputModel editRoleInputModel)
        {
            try
            {
                if (!this.ModelState.IsValid)
                {
                    return this.View(editRoleInputModel);
                }

                bool doesRoleExists = await this.roleService.CheckIfRoleAlreadyExistsByIdAsync(editRoleInputModel.Id);

                if (!doesRoleExists)
                {
                    throw new InvalidOperationException(ExceptionConstants.ApplicationRoleErrors.InvalidRoleId);
                }

                await this.roleService.EditRoleAsync(editRoleInputModel.Id, editRoleInputModel);

                this.TempData[SuccessMessage] = "You successfully edited a role!";

                await this.notificationService.CreateNotificationAsync(
                   $"You successfully edited role: {editRoleInputModel.Name}",
                   "/Admin/Role/Active",
                   this.GetUserId());
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = "Something went wrong!";
            }
            
            return this.RedirectToAction(nameof(Active));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string roleId)
        {
            try
            {
                bool doesRoleExists = await this.roleService.CheckIfRoleAlreadyExistsByIdAsync(roleId);

                if (!doesRoleExists)
                {
                    throw new InvalidOperationException(ExceptionConstants.ApplicationRoleErrors.InvalidRoleId);
                }

                await this.roleService.DeleteRoleAsync(roleId);

                this.TempData[SuccessMessage] = "You successfully deleted a role!";

                await this.notificationService.CreateNotificationAsync(
                    $"You successfully deleted a role!",
                    "/Admin/Role/Deleted",
                    this.GetUserId()); 
                
                return this.RedirectToAction(nameof(Deleted));
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction(nameof(Active));
            }
        }
    }
}
