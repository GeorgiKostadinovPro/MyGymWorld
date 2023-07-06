namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Managers;
    using System.Threading.Tasks;

    public interface IManagerService
    {
        Task<BecomeManagerInputModel> GetUserToBecomeManagerByIdAsync(string userId);
    }
}
