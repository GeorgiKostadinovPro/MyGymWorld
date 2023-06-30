namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Users;

    public interface IUserService
    {
        Task CreateUserAsync(CreateUserInputModel createUserInputModel);
    }
}
