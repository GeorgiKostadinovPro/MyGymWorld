namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.WebUtilities;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;
    using System.Text;
    using System.Threading.Tasks;

    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IMapper mapper;

        public UserService(
            UserManager<ApplicationUser> _userManager, 
            IMapper _mapper)
        {
            this.userManager = _userManager;
            this.mapper = _mapper;
        }

        public async Task<(ApplicationUser, IdentityResult)> CreateUserAsync(CreateUserInputModel createUserInputModel)
        {
            ApplicationUser userToCreate = this.mapper.Map<ApplicationUser>(createUserInputModel);

            userToCreate.CreatedOn = DateTime.UtcNow;

            IdentityResult result = await this.userManager.CreateAsync(userToCreate, createUserInputModel.Password);

            return (userToCreate , result);
        }
        
        public async Task<string> GenerateUserEmailConfirmationTokenAsync(ApplicationUser user)
        {
            string emailConfirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

            byte[] emailConfirmationTokenBytes = Encoding.UTF8.GetBytes(emailConfirmationToken);

            string validEmailToken = WebEncoders.Base64UrlEncode(emailConfirmationTokenBytes);

            return validEmailToken;
        } 
        
        public async Task<bool> CheckIfUserHasConfirmedEmailAsync(string userId)
        {
            ApplicationUser user = await this.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException(ExceptionConstants.ConfimEmail.InvalidUserId);
            }

            return user.EmailConfirmed;
        }
        
        public async Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password)
        {
            bool result = await this.userManager.CheckPasswordAsync(user, password);

            return result;
        }
        
        public async Task<bool> CheckIfUserExistsByEmailAsync(string email)
        {
            ApplicationUser user = await this.GetUserByEmailAsync(email);

            return user != null ? true : false;
        }
        
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByIdAsync(userId);

            return applicationUser;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByEmailAsync(email);

            return applicationUser; 
        }

        public async Task<ApplicationUser> GetUserByUserNameAsync(string username)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByNameAsync(username);

            return applicationUser;
        }
    }
}