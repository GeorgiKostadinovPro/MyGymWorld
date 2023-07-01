namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IUserService
    {
        Task<(ApplicationUser, IdentityResult)> CreateUserAsync(CreateUserInputModel createUserInputModel);

        Task<ApplicationUser> GetUserByIdAsync(string userId);

        Task<ApplicationUser> GetUserByUserNameAsync(string username);

        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task<string> GenerateUserEmailConfirmationTokenAsync(ApplicationUser user);

        Task<bool> CheckIfUserExistsByEmailAsync(string email);

        Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password);
    }
}
