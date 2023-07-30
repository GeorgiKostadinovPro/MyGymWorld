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

        Task<Membership> DeleteMembershipAsync(string membershipId);

        Task BuyMembershipAsync(string membershipId, string userId);

        Task<IEnumerable<MembershipViewModel>> GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsync(string userId, AllUserMemberhipsQueryModel queryModel);

        Task<int> GetAllUserMembershipsCountByUserIdAsync(string userId);

        Task<IEnumerable<MembershipViewModel>> GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(AllMembershipsForGymQueryModel queryModel);

        Task<int> GetAllActiveMembershipsCountByGymIdAsync(string gymId);

        Task<MembershipDetailsViewModel> GetMembershipDetailsByIdAsync(string membershipId);	
        
        Task<EditMembershipInputModel> GetMembershipForEditByIdAsync(string membershipId);

        Task<Membership?> GetMembershipByIdAsync(string membershipId);

        Task<UserMembership?> GetUserMembershipAsync(string userId, string membershipId);


		Task<bool> CheckIfMembershipExistsByIdAsync(string membershipId);

        Task<int> GetAllActiveMembershipsCountAsync();

        IEnumerable<string> GetAllMembershipTypes();
    }
}
