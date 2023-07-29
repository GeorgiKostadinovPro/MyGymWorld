﻿namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
	using MyGymWorld.Web.ViewModels.Managers.Memberships;
	using MyGymWorld.Web.ViewModels.Memberships;
    using System.Threading.Tasks;

    public interface IMembershipService
    {
        Task<Membership> CreateMembershipAsync(CreateMembershipInputModel createMembershipInputModel);

		Task<Membership> EditMembershipAsync(string membershipId, EditMembershipInputModel editMembershipInputModel);

		Task<IEnumerable<MembershipViewModel>> GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(AllMembershipsForGymQueryModel queryModel);

        Task<int> GetAllActiveMembershipsCountByGymIdAsync(string gymId);

        Task<MembershipDetailsViewModel> GetMembershipDetailsByIdAsync(string membershipId);	
        
        Task<EditMembershipInputModel> GetMembershipForEditByIdAsync(string membershipId);

        Task<Membership?> GetMembershipByIdAsync(string membershipId);

        Task<bool> CheckIfMembershipExistsByIdAsync(string membershipId);

		IEnumerable<string> GetAllMembershipTypes();
    }
}
