namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IUserService
    {
        Task<(ApplicationUser, IdentityResult)> CreateUserAsync(CreateUserInputModel createUserInputModel);

        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task<bool> CheckIfUserExistsByEmailAsync(string email);
    }
}
