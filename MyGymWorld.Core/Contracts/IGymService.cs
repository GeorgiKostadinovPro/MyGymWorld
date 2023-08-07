namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Gyms;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System.Threading.Tasks;

    public interface IGymService
    {
        Task CreateGymAsync(Guid managerId, CreateGymInputModel createGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        Task EditGymAsync(string gymId, EditGymInputModel editGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        Task DeleteGymAsync(string gymId);

        Task AddGymToUserAsync(string gymId, string userId);

        Task RemoveGymFromUserAsync(string gymId, string userId);

        Task<EditGymInputModel> GetGymForEditByIdAsync(string gymId);

		Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(Guid managerId, bool isDeleted, int skip = 0, int? take = null);

        Task<List<GymViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null);

        Task<int> GetActiveOrDeletedGymsCountForManagementAsync(Guid managerId, bool isDeleted);

        Task<int> GetActiveOrDeletedGymsCountForAdministrationAsync(bool isDeleted);

        Task<int> GetActiveGymsCountAsync();

        Task<IEnumerable<DisplayGymViewModel>> GetTop10NewestActiveGymsAsync();

        Task<IEnumerable<DisplayGymViewModel>> GetTop10MostLikedActiveGymsAsync();

        Task<IEnumerable<DisplayGymViewModel>> GetAllActiveFilteredAndPagedGymsAsync(AllGymsQueryModel queryModel);

        Task<IEnumerable<DisplayGymViewModel>> GetAllUserJoinedGymsFilteredAndPagedAsync(string userId, AllUserJoinedGymsQueryModel queryModel);

        Task<IEnumerable<ApplicationUser>> GetAllUsersWhoAreSubscribedForGymArticlesAsync(string gymId);

        Task<GymDetailsViewModel> GetGymDetailsByIdAsync(string gymId);

        Task<int> GetAllUserJoinedGymsCountAsync(string userId);

        Task<bool> CheckIfGymExistsByIdAsync(string gymId);

        Task<bool> CheckIfGymIsManagedByManagerAsync(string gymId, string mananerId);

        Task<bool> CheckIfGymIsJoinedByUserAsync(string gymId, string userId);

        Task<Gym?> GetGymByIdAsync(string gymId);

        Task<bool> CheckIfUserIsSubscribedForGymArticles(string userId, string gymId);

        IEnumerable<string> GetAllGymTypes();
    }
}