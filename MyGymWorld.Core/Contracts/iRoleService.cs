namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoleService
    {
        Task AddRoleToUserAsync(string userId, string roleName);

        Task RemoveRoleFromUserAsync(string userId, string roleName);

        Task<List<RoleViewModel>> GetActiveForAdministrationAsync();

        Task<List<RoleViewModel>> GetDeletedForAdministrationAsync();

        Task<bool> CheckIfUserIsInRoleAsync(string userId, string roleName);

        IEnumerable<string> GetAllRoles();
    }
}
