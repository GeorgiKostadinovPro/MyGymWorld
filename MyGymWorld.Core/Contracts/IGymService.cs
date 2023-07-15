namespace MyGymWorld.Core.Contracts
{
    using CloudinaryDotNet.Actions;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System.Threading.Tasks;

    public interface IGymService
    {
        Task CreateGymAsync(Guid managerId, CreateGymInputModel createGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel);

        IEnumerable<string> GetAllGymTypes();
    }
}
