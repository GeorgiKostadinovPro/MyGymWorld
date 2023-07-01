﻿namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Configuration;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Exceptions;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Users;
    using System;
    using System.Text;
    using System.Threading.Tasks;

    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IEmailSenderService emailSenderService;
        private readonly IConfiguration configuration;
        private readonly IRepository repository;

        public AccountService(
            UserManager<ApplicationUser> _userManager, 
            SignInManager<ApplicationUser> signInManager,
            IUserService _userService,
            IMapper _mapper,
            IEmailSenderService _emailSenderService,
            IConfiguration _configuration,
            IRepository _repository)
        {
            this.userManager = _userManager;
            this.signInManager = signInManager;
            this.userService = _userService;
            this.mapper = _mapper;
            this.emailSenderService = _emailSenderService;
            this.configuration = _configuration;
            this.repository = _repository;
        }

        public async Task RegisterUserAsync(RegisterUserInputModel registerUserInputModel)
        {
            bool doesUserExist = await this.userService.CheckIfUserExistsByEmailAsync(registerUserInputModel.Email);

            if (doesUserExist)
            {
                throw new InvalidOperationException(ExceptionConstants.RegisterUser.EmailAlreadyExists);
            }

            CreateUserInputModel userToCreate = this.mapper.Map<CreateUserInputModel>(registerUserInputModel);

            var tuppleResult = await this.userService.CreateUserAsync(userToCreate);

            ApplicationUser user = tuppleResult.Item1;
            IdentityResult result = tuppleResult.Item2;

            if (!result.Succeeded)
            {
                throw new RegisterUserException(result.Errors);
            }

            await this.signInManager.SignInAsync(user, isPersistent: false);

            string emailConfirmationToken = await this.userService.GenerateUserEmailConfirmationTokenAsync(user);

            await this.SendUserEmailConfirmationAsync(user, emailConfirmationToken);
        }
        
        public async Task AuthenticateAsync(LoginUserInputModel loginUserInputModel)
        {
            ApplicationUser user = await this.userService.GetUserByEmailAsync(loginUserInputModel.Email);

            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.RegisterUser.UserDoesNotExist);
            }

            bool doesPasswordMatch = await this.userService.CheckUserPasswordAsync(user, loginUserInputModel.Password);

            if (!doesPasswordMatch)
            {
                throw new InvalidOperationException(ExceptionConstants.LoginUser.InvalidLoginAttempt);
            }

            bool isPersistent = loginUserInputModel.RememberMe;

            SignInResult result = await this.signInManager.PasswordSignInAsync(user, loginUserInputModel.Password, isPersistent: isPersistent, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(ExceptionConstants.LoginUser.InvalidLoginAttempt);
            }

            /*await this.emailSenderService.SendEmailAsync(user.Email, "Successful login", "<h1>Hi, new login to your account was noticed!</h1>" +
                $"<p>New login to your account at {DateTime.UtcNow}</p>");*/
        }

        public async Task LogoutUserAsync()
        {
            await this.signInManager.SignOutAsync();
        }

        public async Task SendUserEmailConfirmationAsync(ApplicationUser user, string emailConfirmationToken)
        {
            string urlForVerification = $"{this.configuration["ApplicationUrl"]}/Account/ConfirmEmail?userId={user.Id}&emailConfirmationToken={emailConfirmationToken}";

            await this.emailSenderService.SendEmailAsync(user.Email,
                "Email confirmation", "<h1>Please, confirm your email address!</h1>" +
                $"<p>Please, click <a href='{urlForVerification}'>here</a></p>");
        }

        public async Task ConfirmUserEmailAsync(string userId, string emailConfirmationToken)
        {
            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new ArgumentException(ExceptionConstants.ConfimEmail.InvalidUserId);
            }

            byte[] decodedToken = WebEncoders.Base64UrlDecode(emailConfirmationToken);

            string originalToken = Encoding.UTF8.GetString(decodedToken);

            IdentityResult result = await this.userManager.ConfirmEmailAsync(user, originalToken);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(ExceptionConstants.ConfimEmail.ConfirmEmailFailed);
            }
        }

        public async Task SendUserResetPasswordEmailAsync(string email)
        {
            ApplicationUser user = await this.userService.GetUserByEmailAsync(email);

            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.ResetPassword.InvalidEmailAddress);
            }

            string token = await this.userManager.GeneratePasswordResetTokenAsync(user);

            string resetUrl = $"{this.configuration["ApplicationUrl"]}/Account/ResetPassword?email={Uri.EscapeDataString(email!)}&token={Uri.EscapeDataString(token)}";

            await this.emailSenderService.SendEmailAsync(user.Email, 
                "Reset password", "<h1>Please, confirm your email address!</h1>" +
                $"<p>Please, click <a href='{resetUrl}'>here</a> to reset your password!</p>");
        }

        public async Task ResetUserPasswordAsync(ResetPasswordInputModel resetPasswordInputModel)
        {
            ApplicationUser user = await this.userService.GetUserByEmailAsync(resetPasswordInputModel.Email);

            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.ResetPassword.InvalidEmailAddress);
            }

            IdentityResult result = await this.userManager.ResetPasswordAsync(user, resetPasswordInputModel.Token, resetPasswordInputModel.Password);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException(ExceptionConstants.ResetPassword.InvalidTokenOrPassword);
            }
        }
    }
}