namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Web.ViewModels.Users;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService _userService)
        {
            this.userService = _userService;
        }

        [HttpGet]
        public async Task<IActionResult> UserProfile()
        {
            string userId = this.GetUserId();

            try
            {
                UserProfileViewModel userProfileViewModel = await this.userService.GetUserToDisplayByIdAsync(userId);
                
                return this.View(userProfileViewModel);
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction("Index", "Home");
            }
        }
    }
}
