namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Memberships;
    using MyGymWorld.Web.ViewModels.Memberships;
    using System.Threading.Tasks;

    public interface IMembershipService
    {
        Task<Membership> CreateMembershipAsync(CreateMembershipInputModel createMembershipInputModel);

        Task<IEnumerable<MembershipViewModel>> GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(AllMembershipsForGymQueryModel queryModel);

        Task<int> GetAllActiveMembershipsCountByGymIdAsync(string gymId);

        IEnumerable<string> GetAllMembershipTypes();
    }
}
