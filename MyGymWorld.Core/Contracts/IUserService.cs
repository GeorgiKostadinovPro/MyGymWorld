namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IUserService
    {
        Task<(ApplicationUser, IdentityResult)> CreateUserAsync(CreateUserInputModel createUserInputModel);

        Task<(ApplicationUser, IdentityResult)> EditUserAsync(string userId, EditUserInputModel editUserInputModel);

        Task UploadUserProfilePictureAsync(ApplicationUser user, IFormFile profilePicture);

        Task<ApplicationUser> GetUserByIdAsync(string userId);

        Task<ApplicationUser> GetUserByUsernameAsync(string username);

        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task<EditUserInputModel> GetUserForEditByIdAsync(string userId);

        Task<UserProfileViewModel> GetUserToDisplayByIdAsync(string userId);

        Task<string> GenerateUserEmailConfirmationTokenAsync(ApplicationUser user);

        Task<string> GenerateUserPasswordResetTokenAsync(ApplicationUser user);


        Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password);

        Task<bool> CheckIfUserExistsByEmailAsync(string email);

        Task<bool> CheckIfUserExistsByUsernameAsync(string username);


        Task<bool> CheckIfUserHasConfirmedEmailAsync(string userId);
    }
}