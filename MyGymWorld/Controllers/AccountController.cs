namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Server.IIS.Core;
    using Microsoft.EntityFrameworkCore.Internal;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Exceptions;
    using MyGymWorld.Web.ViewModels.Users;

    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService _accountService)
        {
            this.accountService = _accountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            RegisterUserInputModel registerUserInputModel = new RegisterUserInputModel
            {
                ReturnUrl = returnUrl
            };

            return View(registerUserInputModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUserInputModel registerUserInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(registerUserInputModel);
            }

            try
            {
                await this.accountService.RegisterUserAsync(registerUserInputModel);

                return this.RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                this.ModelState.AddModelError("Email", ex.Message);

                return this.View(registerUserInputModel);
            }
            catch (RegisterUserException ex)
            {
                foreach (var error in ex.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.View(registerUserInputModel);
            }
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
