namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Web.ViewModels.Users;

    public class AccountController : BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            RegisterUserInputModel registerUserInputModel = new RegisterUserInputModel
            {
                ReturnUrl = returnUrl
            };

            return View(registerUserInputModel);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            LoginUserInputModel loginUserInputModel = new LoginUserInputModel
            {
                ReturnUrl = returnUrl
            };

            return this.View(loginUserInputModel);
        }

        public async Task<IActionResult> Logout()
        {
            return this.RedirectToAction("Index", "Home");
        }
    }
}
