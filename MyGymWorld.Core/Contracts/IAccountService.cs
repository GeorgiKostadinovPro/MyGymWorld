namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IAccountService
    {
        Task RegisterUserAsync(RegisterUserInputModel registerUserInputModel);

        Task AuthenticateAsync(LoginUserInputModel loginUserInputModel);
        
        Task SendUserEmailConfirmationAsync(ApplicationUser user, string emailConfirmationToken);

        Task ConfirmUserEmailAsync(string userId, string emailConfirmationToken);

        Task LogoutUserAsync();
        
        Task<IList<AuthenticationScheme>> GetExternalLoginsAsync();

        AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl);

        Task<ExternalLoginInfo> GetExternalLoginInfoAsync();

        Task<SignInResult> ExternalLoginSignInAsync(string loginProvider, string providerKey);
    }
}