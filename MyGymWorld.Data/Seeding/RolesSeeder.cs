using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyGymWorld.Data.Common.Constants;
using MyGymWorld.Data.Models;
using MyGymWorld.Data.Seeding.Contracts;

namespace MyGymWorld.Data.Seeding
{
    public class RolesSeeder : ISeeder
    {
        public async Task SeedAsync(MyGymWorldDbContext dbContext, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            await SeedRoleAsync(roleManager, ApplicationRoleConstants.AdministratorRoleName);
        }

        private static async Task SeedRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
