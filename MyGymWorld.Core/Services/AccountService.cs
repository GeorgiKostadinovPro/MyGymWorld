namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Core.Exceptions;
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

        public AccountService(
            UserManager<ApplicationUser> _userManager, 
            SignInManager<ApplicationUser> signInManager,
            IUserService _userService,
            IMapper _mapper)
        {
            this.userManager = _userManager;
            this.signInManager = signInManager;
            this.userService = _userService;
            this.mapper = _mapper;
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
            throw new NotImplementedException();
        }

        public async Task LogoutUserAsync()
        {
            throw new NotImplementedException();
        }
    }
}
