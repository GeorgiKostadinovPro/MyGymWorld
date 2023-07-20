namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Gyms;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System.Threading.Tasks;

    public interface IGymService
    {
        Task CreateGymAsync(Guid managerId, CreateGymInputModel createGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        Task EditGymAsync(string gymId, EditGymInputModel editGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        Task<EditGymInputModel> GetGymForEditByIdAsync(string gymId);

		Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(Guid managerId, bool isDeleted, int skip = 0, int? take = null);

        Task<List<GymViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null);

        Task<int> GetActiveOrDeletedGymsCountForManagementAsync(Guid managerId, bool isDeleted);

        Task<int> GetActiveOrDeletedGymsCountForAdministrationAsync(bool isDeleted);

        Task<int> GetActiveGymsCountAsync();

        Task<IEnumerable<DisplayGymViewModel>> GetTop10NewestActiveGymsAsync();

        Task<IEnumerable<DisplayGymViewModel>> GetTop10MostLikedActiveGymsAsync();

        Task<IEnumerable<DisplayGymViewModel>> GetAllFilteredAndPagedActiveGymsAsync(AllGymsQueryModel queryModel);

        Task<bool> CheckIfGymExistsByIdAsync(string gymId);

		IEnumerable<string> GetAllGymTypes();
    }
}
