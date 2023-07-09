namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoleService
    {
        Task AddRoleToUserAsync(string userId, string roleName);

        Task RemoveRoleFromUserAsync(string userId, string roleName);

        Task CreateRoleAsync(CreateRoleInputModel createRoleInputModel);

        Task<List<RoleViewModel>> GetActiveForAdministrationAsync();

        Task<List<RoleViewModel>> GetDeletedForAdministrationAsync();
        
        Task<IEnumerable<string>> GetAllRoleNamesAsync();

        Task<bool> CheckIfUserIsInRoleAsync(string userId, string roleName);

        Task<bool> CheckIfRoleAlreadyExistsAsync(string roleName);
    }
}
