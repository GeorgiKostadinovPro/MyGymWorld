namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Users;

    public interface IAccountService
    {
        Task RegisterUserAsync(RegisterUserInputModel registerUserInputModel);

        Task AuthenticateAsync(LoginUserInputModel loginUserInputModel);
        
        Task SendUserEmailConfirmationAsync(ApplicationUser user, string emailConfirmationToken);

        Task ConfirmUserEmailAsync(string userId, string emailConfirmationToken);

        Task LogoutUserAsync();
    }
}