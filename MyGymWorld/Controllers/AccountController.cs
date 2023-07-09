﻿namespace MyGymWorld.Web.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Exceptions;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;
    using Microsoft.AspNetCore.Identity;

    using static MyGymWorld.Common.NotificationMessagesConstants;
    using MyGymWorld.Core.Utilities.Contracts;
    using System.Net.WebSockets;

    public class AccountController : BaseController
    {
        private readonly IAccountService accountService;
        private readonly IUserService userService;

        private readonly IEmailSenderService emailSenderService;

        public AccountController(
            IAccountService _accountService, 
            IUserService _userService,
            IEmailSenderService _emailSenderService)
        {
            this.accountService = _accountService;
            this.userService = _userService;

            this.emailSenderService = _emailSenderService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            if (User != null && User.Identity != null
                && User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

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
                bool doesUserExist = await this.userService.CheckIfUserExistsByEmailAsync(registerUserInputModel.Email);

                if (doesUserExist)
                {
                    throw new InvalidOperationException(ExceptionConstants.RegisterUser.EmailAlreadyExists);
                }

                (ApplicationUser user, IdentityResult result) = await this.accountService.RegisterUserAsync(registerUserInputModel);

                if (!result.Succeeded)
                {
                    throw new RegisterUserException(result.Errors);
                }

                string emailConfirmationToken = await this.userService.GenerateUserEmailConfirmationTokenAsync(user);

                await accountService.SendUserEmailConfirmationAsync(user, emailConfirmationToken);

                this.TempData[InformationMessage] = "A confirmation email was sent to you! Please, confirm your account!";

                return this.RedirectToAction(nameof(Login));
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
        public IActionResult Login(string returnUrl = null!)
        {
            if (User != null && User.Identity != null
                && User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

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
                ApplicationUser user = await this.userService.GetUserByEmailAsync(loginUserInputModel.Email);

                if (user == null)
                {
                    throw new InvalidOperationException(ExceptionConstants.LoginUser.UserDoesNotExist);
                }

                if (user.IsDeleted == true)
                {
                    throw new InvalidOperationException(ExceptionConstants.LoginUser.UserWasDeletedByAdmin);
                }

                bool doesPasswordMatch = await this.userService.CheckUserPasswordAsync(user, loginUserInputModel.Password);

                if (!doesPasswordMatch)
                {
                    throw new InvalidOperationException(ExceptionConstants.LoginUser.InvalidLoginAttempt);
                }

                var result =  await this.accountService.AuthenticateAsync(loginUserInputModel);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException(ExceptionConstants.LoginUser.InvalidLoginAttempt);
                }

                //await this.emailSenderService.SendEmailAsync(user.Email, "Successful login", "<h1>Hi, new login to your account was noticed!</h1>" +
                //$"<p>New login to your account at {DateTime.UtcNow}</p>");

                return this.RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;
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
            if (User != null && User.Identity != null
                && User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

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
            catch (Exception ex)
            {
                this.TempData[ErrorMessage] = ex.Message;

                return this.RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SendResetPasswordEmail()
        {
            if (User != null && User.Identity != null
                && User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

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
            if (User != null && User.Identity != null
                && User.Identity.IsAuthenticated)
            {
                return this.RedirectToAction("Index", "Home");
            }

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