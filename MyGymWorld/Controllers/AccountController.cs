namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication.Google;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Exceptions;
    using MyGymWorld.Web.ViewModels.Users;

    using static MyGymWorld.Common.NotificationMessagesConstants;

    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;
        private readonly IUserService userService;

        public AccountController(
            IAccountService _accountService, 
            IUserService _userService)
        {
            this.accountService = _accountService;
            this.userService = _userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
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

                this.TempData[InformationMessage] = "A confirmation email was sent to you! Please, confirm your account!";

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

        [HttpGet]  
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            LoginUserInputModel loginUserInputModel = new LoginUserInputModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = await this.accountService.GetExternalLoginsAsync()
            };

            return this.View(loginUserInputModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUserInputModel loginUserInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(loginUserInputModel);
            }

            try
            {
                await this.accountService.AuthenticateAsync(loginUserInputModel);

                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);

                return this.View(loginUserInputModel);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await this.accountService.LogoutUserAsync();

            return this.RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string emailConfirmationToken)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                return this.BadRequest("Invalid confirmation parameters!");
            }

            try
            {
                await this.accountService.ConfirmUserEmailAsync(userId, emailConfirmationToken);

                this.TempData[SuccessMessage] = "Successfully confirmed email!";

                return this.RedirectToAction("Index", "Home");
            }
            catch (ArgumentException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction("Index", "Home");
            }
            catch (InvalidOperationException ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", values: new { returnUrl });
            var properties = this.accountService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (remoteError != null)
            {
                this.TempData[ErrorMessage] = $"Error from external provider: {remoteError}";

                return this.RedirectToAction("Login", new { ReturnUrl = returnUrl });
            }
            var info = await this.accountService.GetExternalLoginInfoAsync();

            if (info == null)
            {
                this.TempData[ErrorMessage] = "Error loading external login information.";

                return RedirectToPage("Login", new { ReturnUrl = returnUrl });
            }

            var result = await this.accountService.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey);

            if (result.Succeeded)
            {
                return this.LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                TempData[InformationMessage] = "Please create an account before login!";

                return this.RedirectToAction(nameof(Register));
            }
        }
    }
}