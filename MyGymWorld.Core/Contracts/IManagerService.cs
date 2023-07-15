namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Administration.Managers;
    using MyGymWorld.Web.ViewModels.Managers;
    using System.Threading.Tasks;

    public interface IManagerService
    {
        Task CreateManagerAsync(string userId, BecomeManagerInputModel becomeManagerInputModel);

        Task<Manager> ApproveManagerAsync(string managerId, string adminId);

        Task<Manager> RejectManagerAsync(string managerId, string adminId);

        Task<IEnumerable<ManagerRequestViewModel>> GetAllNotApprovedManagerRequestsAsync();

        Task<int> GetAllNotApprovedManagerRequestsCountAsync();

        Task<ManagerRequestViewModel> GetSingleManagerRequestByManagerIdAsync(string managerId);

        Task<BecomeManagerInputModel> GetUserToBecomeManagerByIdAsync(string userId);

        Task<bool> CheckIfUserIsAManagerAsync(string userId);

        Task<bool> CheckIfManagerExistsByPhoneNumberAsync(string phoneNumber);

        IEnumerable<string> GetAllManagerTypes();

        Task<Manager> GetManagerByIdAsync(string userId);
    }  
}
