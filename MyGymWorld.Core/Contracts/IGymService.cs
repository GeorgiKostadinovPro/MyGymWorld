namespace MyGymWorld.Core.Contracts
{
    using CloudinaryDotNet.Actions;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System.Threading.Tasks;

    public interface IGymService
    {
        Task CreateGymAsync(Guid managerId, CreateGymInputModel createGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(bool isDeleted, int skip = 0, int? take = null);

        Task<int> GetActiveOrDeletedGymsCountAsync(bool isDeleted);

        IEnumerable<string> GetAllGymTypes();
    }
}
