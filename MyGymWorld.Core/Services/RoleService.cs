namespace MyGymWorld.Core.Services
{
    using Microsoft.AspNetCore.Identity;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        
        public RoleService(
            RoleManager<ApplicationRole> _roleManager, 
            UserManager<ApplicationUser> _userManager)
        {
            this.roleManager = _roleManager;
            this.userManager = _userManager;
        }

        public async Task AddRoleToUserAsync(string userId, string roleName)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            await this.userManager.AddToRoleAsync(user, roleName);
        }

        public async Task RemoveRoleFromUserAsync(string userId, string roleName)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            await this.userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<bool> CheckIfUserIsInRoleAsync(string userId, string roleName)
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            return await this.userManager.IsInRoleAsync(user, roleName);
        }

        public IEnumerable<string> GetAllRoles()
        {
            return this.roleManager.Roles
                                   .ToArray()
                                   .Select(r => r.Name);
        }
    }
}
