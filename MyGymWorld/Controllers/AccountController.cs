namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
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

                this.TempData[InformationMessage] = "A confirmation email was sent to you! Please, confirm your account!";

                return this.View(registerUserInputModel);
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
        public IActionResult Login(string returnUrl = null)
        {
            LoginUserInputModel loginUserInputModel = new LoginUserInputModel
            {
                ReturnUrl = returnUrl
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

                return this.RedirectToAction(nameof(Login));
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

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendResetPasswordEmail()
        {
            EnterEmailInputModel model = new EnterEmailInputModel();

            return this.View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendResetPasswordEmail(EnterEmailInputModel enterEmailInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(enterEmailInputModel);
            }

            try
            {
                await this.accountService.SendUserResetPasswordEmailAsync(enterEmailInputModel.Email);

                this.TempData[InformationMessage] = "Reset password link was sent to your email!";

                return this.View(enterEmailInputModel);
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.View(enterEmailInputModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            ResetPasswordInputModel model = new ResetPasswordInputModel
            {
                Email = email,
                Token = token
            };

            return this.View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordInputModel resetPasswordInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(resetPasswordInputModel);
            }

            try
            {
                await this.accountService.ResetUserPasswordAsync(resetPasswordInputModel);

                this.TempData[SuccessMessage] = "You succesfully reset your password!";

                return this.RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {

                this.TempData[ErrorMessage] = ex.Message;

                return this.View(resetPasswordInputModel);
            }
        }
    }
}