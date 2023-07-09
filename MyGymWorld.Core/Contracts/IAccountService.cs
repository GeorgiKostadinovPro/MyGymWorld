namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IAccountService
    {
        Task<(ApplicationUser, IdentityResult)> RegisterUserAsync(RegisterUserInputModel registerUserInputModel);

        Task<SignInResult> AuthenticateAsync(LoginUserInputModel loginUserInputModel);
        
        Task SendUserEmailConfirmationAsync(ApplicationUser user, string emailConfirmationToken);

        Task ConfirmUserEmailAsync(string userId, string emailConfirmationToken);

        Task SendUserResetPasswordEmailAsync(string email);

        Task ResetUserPasswordAsync(ResetPasswordInputModel resetPasswordInputModel);

        Task LogoutUserAsync();
    }
}