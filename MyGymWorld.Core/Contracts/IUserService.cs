namespace MyGymWorld.Core.Contracts
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Administration.Users;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IUserService
    {
        Task<(ApplicationUser, IdentityResult)> CreateUserAsync(CreateUserInputModel createUserInputModel);

        Task<(ApplicationUser, IdentityResult)> EditUserAsync(string userId, EditUserInputModel editUserInputModel);

        Task DeleteUserAsync(string userId);

        Task SetUserProfilePictureAsync(string userId, ImageUploadResult imageUploadResult);

        Task DeleteUserProfilePictureAsync(string userId);

        Task<(string, string)> GetUserProfilePictureUriAndPublicIdAsync(string userId);

        Task<ApplicationUser> GetUserByIdAsync(string userId);

        Task<ApplicationUser> GetUserByUsernameAsync(string username);

        Task<ApplicationUser> GetUserByEmailAsync(string email);

        Task<EditUserInputModel> GetUserForEditByIdAsync(string userId);

        Task<UserProfileViewModel> GetUserToDisplayByIdAsync(string userId);

        Task<List<UserViewModel>> GetActiveForAdministrationAsync();

        Task<List<UserViewModel>> GetDeletedForAdministrationAsync();

        Task<ApplicationUser> GetAdministratorAsync();

        Task<IEnumerable<ApplicationUser>> GetAllAsync();

        Task<string> GenerateUserEmailConfirmationTokenAsync(ApplicationUser user);

        Task<string> GenerateUserPasswordResetTokenAsync(ApplicationUser user);


        Task<bool> CheckUserPasswordAsync(ApplicationUser user, string password);

        Task<bool> CheckIfUserExistsByEmailAsync(string email);

        Task<bool> CheckIfUserExistsByUsernameAsync(string username);

        Task<bool> CheckIfUserExistsByPhoneNumberAsync(string phoneNumber);

        Task<bool> CheckIfUserHasConfirmedEmailAsync(string userId);
    }
}