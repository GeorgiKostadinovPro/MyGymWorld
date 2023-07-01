﻿namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Exceptions;
    using MyGymWorld.Core.Utilities.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;
    using System;
    using System.Threading.Tasks;

    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        private readonly IUserService userService;
        private readonly IMapper mapper;
        private readonly IEmailSenderService emailSenderService;

        public AccountService(
            UserManager<ApplicationUser> _userManager, 
            SignInManager<ApplicationUser> signInManager,
            IUserService _userService,
            IMapper _mapper,
            IEmailSenderService _emailSenderService)
        {
            this.userManager = _userManager;
            this.signInManager = signInManager;
            this.userService = _userService;
            this.mapper = _mapper;
            this.emailSenderService = _emailSenderService;
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

            await this.emailSenderService.SendEmailAsync(user.Email, "Successful login", "<h1>Hi, new login to your account was noticed!</h1>" +
                $"<p>New login to your account at {DateTime.UtcNow}</p>");
        }

        public async Task LogoutUserAsync()
        {
            await this.signInManager.SignOutAsync();
        }
    }
}