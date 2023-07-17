namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System.Threading.Tasks;

    public interface IGymService
    {
        Task CreateGymAsync(Guid managerId, CreateGymInputModel createGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(Guid managerId, bool isDeleted, int skip = 0, int? take = null);

        Task<List<GymViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null);

        Task<int> GetActiveOrDeletedGymsCountByManagerIdAsync(Guid managerId, bool isDeleted);

        Task<int> GetActiveOrDeletedGymsCountForAdministrationAsync(bool isDeleted);

        IEnumerable<string> GetAllGymTypes();
    }
}
