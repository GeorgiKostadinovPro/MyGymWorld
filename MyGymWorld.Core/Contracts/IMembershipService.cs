namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Memberships;
    using System.Threading.Tasks;

    public interface IMembershipService
    {
        Task<Membership> CreateMembershipAsync(CreateMembershipInputModel createMembershipInputModel);

        IEnumerable<string> GetAllMembershipTypes();
    }
}
