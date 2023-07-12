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

        public async Task CreateRoleAsync(CreateRoleInputModel createRoleInputModel)
        {
            ApplicationRole role = new ApplicationRole(createRoleInputModel.Name);

            role.CreatedOn = DateTime.UtcNow;

            await this.roleManager.CreateAsync(role);
        }

        public async Task EditRoleAsync(string roleId, EditRoleInputModel editRoleInputModel)
        {
            ApplicationRole role = await this.roleManager.FindByIdAsync(roleId);

            role.Name = editRoleInputModel.Name;
            role.ModifiedOn = DateTime.UtcNow;

            await this.roleManager.UpdateAsync(role);
        }

        public async Task<ApplicationRole> DeleteRoleAsync(string roleId)
        {
            ApplicationRole role = await this.repository.GetByIdAsync<ApplicationRole>(Guid.Parse(roleId));

            role.IsDeleted = true;
            role.DeletedOn = DateTime.UtcNow;

            foreach (var user in await this.userManager.Users.ToArrayAsync())
            {
                bool isInRole = await this.userManager.IsInRoleAsync(user, role.Name);

                if (!isInRole)
                {
                    continue;
                }

                await this.userManager.RemoveFromRoleAsync(user, role.Name);
            }

            await this.repository.SaveChangesAsync();

            return role;
        }

        public async Task<List<RoleViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null)
        {
            IQueryable<ApplicationRole> rolesAsQuery = this.repository.AllReadonly<ApplicationRole>(r => r.IsDeleted == isDeleted)
                .OrderByDescending(u => u.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                rolesAsQuery = rolesAsQuery.Take(take.Value);
            }

            return await rolesAsQuery
                .ProjectTo<RoleViewModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<int> GetActiveOrDeletedRolesCount(bool isDeleted)
        {
            return await this.repository.AllReadonly<ApplicationRole>(r => r.IsDeleted == isDeleted)
                .CountAsync();
        }

        public async Task<IEnumerable<string>> GetAllRoleNamesAsync()
        {
            return await this.repository.AllReadonly<ApplicationRole>()
                .Select(r => r.Name)
                .ToListAsync();
        } 
        
        public async Task<EditRoleInputModel> GetRoleForEditAsync(string roleId)
        {
            ApplicationRole role  = await this.roleManager.FindByIdAsync(roleId);

            return this.mapper.Map<EditRoleInputModel>(role);
        }
        
        public async Task<bool> CheckIfUserIsInRoleAsync(string userId, string roleName)    
        {
            ApplicationUser user = await this.userManager.FindByIdAsync(userId);

            return await this.userManager.IsInRoleAsync(user, roleName);
        }
        
        public async Task<bool> CheckIfRoleAlreadyExistsByIdAsync(string roleId)
        {
            ApplicationRole role = await this.roleManager.FindByIdAsync(roleId);

            return role != null ? true : false;
        }

        public async Task<bool> CheckIfRoleAlreadyExistsByNameAsync(string roleName)
        {
            string wildCard = $"%{roleName.ToLower()}%";

            bool result = await this.repository.AllReadonly<ApplicationRole>()
                .AnyAsync(r => EF.Functions.Like(r.Name, wildCard));

            return result;
        }
    }
}