namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;
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

            IdentityResult result = await this.userManager.CreateAsync(userToCreate);

            return (userToCreate , result);
        }
        
        public async Task<bool> CheckIfUserExistsByEmailAsync(string email)
        {
            ApplicationUser user = await this.GetUserByEmailAsync(email);

            return user != null ? true : false;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            ApplicationUser applicationUser = await this.userManager
               .FindByEmailAsync(email);

            return applicationUser; 
        }
    }
}