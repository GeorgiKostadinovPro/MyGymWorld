namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System.Threading.Tasks;

    public interface IGymService
    {
        Task CreateGymAsync(CreateGymInputModel createGymInputModel);
    }
}
