namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoleService
    {
        Task AddRoleToUserAsync(string userId, string roleName);

        Task RemoveRoleFromUserAsync(string userId, string roleName);

        Task CreateRoleAsync(CreateRoleInputModel createRoleInputModel);

        Task EditRoleAsync(string roleId, EditRoleInputModel editRoleInputModel);

        Task DeleteRoleAsync(string roleId);

        Task<List<RoleViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null);

        Task<int> GetActiveOrDeletedRolesCountAsync(bool isDeleted);

        Task<EditRoleInputModel> GetRoleForEditAsync(string roleId);

        Task<bool> CheckIfUserIsInRoleAsync(string userId, string roleName);

        Task<bool> CheckIfRoleAlreadyExistsByIdAsync(string roleId);

        Task<bool> CheckIfRoleAlreadyExistsByNameAsync(string roleName);
    }
}
