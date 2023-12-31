﻿namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
	using MyGymWorld.Web.ViewModels.Managers.Memberships;
	using MyGymWorld.Web.ViewModels.Memberships;
    using System.Threading.Tasks;

    public interface IMembershipService
    {
        Task<Membership> CreateMembershipAsync(CreateMembershipInputModel createMembershipInputModel);

		Task EditMembershipAsync(string membershipId, EditMembershipInputModel editMembershipInputModel);

        Task DeleteMembershipAsync(string membershipId);

        Task BuyMembershipAsync(string membershipId, string userId);

        Task<IEnumerable<MembershipViewModel>> GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsync(string userId, AllUserMemberhipsQueryModel queryModel);

        Task<IEnumerable<MembershipViewModel>> GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(AllMembershipsForGymQueryModel queryModel);
        
        Task<List<PayedMembershipViewModel>> GetActivePaymentsByGymIdForManagementAsync(string gymId, int skip = 0, int? take = null);

        Task<List<PayedMembershipViewModel>> GetActivePaymentsByUserIdAsync(string userId, int skip = 0, int? take = null);

		Task<int> GetAllActiveUserMembershipsCountByUserIdAsync(string userId);

        Task<int> GetAllActiveMembershipsCountByGymIdAsync(string gymId);

        Task<int> GetActivePaymentsCountByGymIdAsync(string gymId);

        Task<MembershipDetailsViewModel> GetMembershipDetailsByIdAsync(string membershipId);	
        
        Task<EditMembershipInputModel> GetMembershipForEditByIdAsync(string membershipId);

        Task<Membership?> GetMembershipByIdAsync(string membershipId);

        Task<UserMembership?> GetUserMembershipAsync(string userId, string membershipId);


		Task<bool> CheckIfMembershipExistsByIdAsync(string membershipId);

        Task<int> GetAllActiveMembershipsCountAsync();

        IEnumerable<string> GetAllMembershipTypes();
    }
}
