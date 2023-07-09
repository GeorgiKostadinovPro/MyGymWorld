namespace MyGymWorld.Web.Areas.Admin.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.Areas.Administration.Controllers;
    using MyGymWorld.Web.ViewModels.Administration.Users;

    public class UserController : AdminController
    {
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            this.userService = _userService;
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            AllUsersViewModel allUsersViewModel = new AllUsersViewModel
            {
                Users = await this.userService.GetAllForAdministrationAsync(),
            };

            return this.View(allUsersViewModel);
        }
    }
}
