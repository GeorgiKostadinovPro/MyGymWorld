namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Web.ViewModels.Managers;
    using System.Threading.Tasks;

    public interface IManagerService
    {
        Task CreateManagerAsync(string userId, BecomeManagerInputModel becomeManagerInputModel);

        Task<BecomeManagerInputModel> GetUserToBecomeManagerByIdAsync(string userId);

        Task<bool> CheckIfUserIsAManagerAsync(string userId);

        Task<bool> CheckIfManagerExistsNyPhoneNumberAsync(string phoneNumber);

        IEnumerable<string> GetAllManagerTypes();
    }
}
