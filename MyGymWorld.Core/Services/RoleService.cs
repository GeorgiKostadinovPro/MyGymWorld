namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Administration.Roles;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        private readonly IRepository repository;
        private readonly IMapper mapper;
        
        public RoleService(
            RoleManager<ApplicationRole> _roleManager, 
            UserManager<ApplicationUser> _userManager,
            IRepository _repository,
            IMapper _mapper)
        {
            this.roleManager = _roleManager;
            this.userManager = _userManager;

            this.repository = _repository;
            this.mapper = _mapper;
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

        public async Task<List<RoleViewModel>> GetActiveForAdministrationAsync()
        {
            return await this.repository.AllReadonly<ApplicationRole>(r => r.IsDeleted == false)
                .ProjectTo<RoleViewModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<List<RoleViewModel>> GetDeletedForAdministrationAsync()
        {
            return await this.repository.AllReadonly<ApplicationRole>(r => r.IsDeleted == true)
                .ProjectTo<RoleViewModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
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
