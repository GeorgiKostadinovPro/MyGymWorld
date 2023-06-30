namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Users;

    public interface IAccountService
    {
        Task RegisterUserAsync(RegisterUserInputModel registerUserInputModel);

        Task AuthenticateAsync(LoginUserInputModel loginUserInputModel);
    }
}
