namespace MyGymWorld.Core.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IRoleService
    {
        Task AddRoleToUserAsync(string userId, string roleName);

        Task RemoveRoleFromUserAsync(string userId, string roleName);

        Task<bool> CheckIfUserIsInRoleAsync(string userId, string roleName);

        IEnumerable<string> GetAllRoles();
    }
}
