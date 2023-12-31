﻿namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Administration.Managers;
    using MyGymWorld.Web.ViewModels.Managers;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManagerService : IManagerService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IUserService userService;
        private readonly IRoleService roleService;
        private readonly INotificationService notificationService;
        
        public ManagerService(
            IMapper _mapper, 
            IRepository repository, 
            IUserService _userService,
            IRoleService _roleService,
            INotificationService _notificationService)
        {
            this.mapper = _mapper;
            this.repository = repository;

            this.userService = _userService;
            this.roleService = _roleService;
            this.notificationService = _notificationService;
        }

        public async Task CreateManagerAsync(string userId, BecomeManagerInputModel becomeManagerInputModel)
        {
            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            if (user.FirstName == null)
            {
                user.FirstName = becomeManagerInputModel.FirstName;
            }

            if (user.LastName == null)
            {
                user.LastName = becomeManagerInputModel.LastName;
            }

            if (user.PhoneNumber == null)
            {
                user.PhoneNumber = becomeManagerInputModel.PhoneNumber;
            }
            
            Manager manager = new Manager
            {
                UserId = Guid.Parse(becomeManagerInputModel.Id),
                ManagerType = Enum.Parse<ManagerType>(becomeManagerInputModel.ManagerType),
                CreatedOn = DateTime.UtcNow
            };
            
            await this.repository.AddAsync(manager);
            await this.repository.SaveChangesAsync();

            user.ManagerId = manager.Id;

            ApplicationUser admin = await this.userService.GetAdministratorAsync();

            await this.notificationService.CreateNotificationAsync(
                "You have succesfully applied for Manager! Admin will approve you soon!", 
                null!, 
                manager.UserId.ToString());

            await this.notificationService.CreateNotificationAsync(
                $"You have received a manager request from {user.FirstName} {user.LastName}",
                $"/Admin/Manager/Requests",
                admin.Id.ToString());
        }

        public async Task<Manager> ApproveManagerAsync(string managerId, string adminId)
        {
            Manager? manager = await this.repository.AllNotDeleted<Manager>()
                .FirstOrDefaultAsync(m => m.Id.ToString() == managerId);

            if (manager == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.ManagerErrors.InvalidManagerId, managerId));
            }

            if (manager.IsRejected)
            {
                manager.IsRejected = false;
            }

            manager.IsApproved = true;
            manager.ModifiedOn = DateTime.UtcNow;

            await this.repository.SaveChangesAsync();

            await this.roleService.AddRoleToUserAsync(manager.UserId.ToString(), ApplicationRoleConstants.ManagerRoleName);

            await this.notificationService.CreateNotificationAsync(
                "Your request was approved by the Admin! You are now a manager!",
                "/User/UserProfile",
                manager.UserId.ToString());

            await this.notificationService.CreateNotificationAsync(
               $"You approved {manager.User.FirstName} {manager.User.LastName}!",
               "Admin/Manager/Requests",
               adminId);

            return manager;
        }

        public async Task<Manager> RejectManagerAsync(string managerId, string adminId)
        {
            Manager? manager = await this.repository.AllNotDeleted<Manager>()
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse(managerId));

            if (manager == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.ManagerErrors.InvalidManagerId, managerId));
            }

            bool wasManagerApprovedBefore = await this.roleService.CheckIfUserIsInRoleAsync(manager.UserId.ToString(), ApplicationRoleConstants.ManagerRoleName);

            manager.IsRejected = true;
            manager.ModifiedOn = DateTime.UtcNow;
            
            if (wasManagerApprovedBefore)
            {  
                manager.IsApproved = false;
                
                await this.roleService.RemoveRoleFromUserAsync(manager.UserId.ToString(), "Manager");

                await this.notificationService.CreateNotificationAsync(
                   "You were removed from Manager position! You are NOT a manager anymore!",
                   "/User/UserProfile",
                   manager.UserId.ToString());

                await this.notificationService.CreateNotificationAsync(
                   $"You rejected {manager.User.FirstName} {manager.User.LastName}! He is NOT a manager anymore!",
                   "Admin/Manager/Requests",
                   adminId);
            }
            else
            {
                await this.notificationService.CreateNotificationAsync(
                   "Your Manager request was rejected by the Admin!",
                   "/User/UserProfile",
                   manager.UserId.ToString());

                await this.notificationService.CreateNotificationAsync(
                   $"You rejected {manager.User.FirstName} {manager.User.LastName} manager request!",
                   "Admin/Manager/Requests",
                   adminId);
            }
            
            await this.repository.SaveChangesAsync();

            return manager;
        }

        public async Task<IEnumerable<ManagerRequestViewModel>> GetAllNotApprovedManagerRequestsAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Manager>()
                .Where(m => m.IsApproved == false && m.IsRejected == false)
                .ProjectTo<ManagerRequestViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<int> GetAllNotApprovedManagerRequestsCountAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Manager>()
                .CountAsync(m => m.IsApproved == false && m.IsRejected == false);
        }

        public async Task<ManagerRequestViewModel> GetSingleManagerRequestByManagerIdAsync(string managerId)
        {
            return await this.repository.AllNotDeletedReadonly<Manager>()
                .Where(m => m.IsApproved == false && m.IsRejected == false && m.Id.ToString() == managerId)
                .ProjectTo<ManagerRequestViewModel>(this.mapper.ConfigurationProvider)
                .FirstAsync();
        }

        public async Task<Manager?> GetManagerForApprovalAndRejectionAsync(string managerId)
        {
            Manager? manager = await this.repository.AllNotDeletedReadonly<Manager>()
                .FirstOrDefaultAsync(m => m.Id.ToString() == managerId);

            return manager;
        }

        public async Task<Manager?> GetManagerByUserIdAsync(string userId)
        {
            Manager? manager = await this.repository.AllNotDeletedReadonly<Manager>()
                .FirstOrDefaultAsync(m => m.UserId.ToString() == userId);

            return manager;
        }
        
        public async Task<Manager?> GetManagerByIdAsync(string managerId)
        {
            Manager? manager = await this.repository.AllNotDeletedReadonly<Manager>()
                .FirstOrDefaultAsync(m => m.Id.ToString() == managerId);

            return manager;
        }

        public async Task<bool> CheckIfUserIsAManagerAsync(string userId)
        {
            bool isManager = await this.repository.AllNotDeletedReadonly<Manager>()
                .AnyAsync(m => m.UserId.ToString() == userId);

            return isManager;
        }

        public async Task<bool> CheckIfManagerExistsByPhoneNumberAsync(string phoneNumber)
        {
            bool existsByPhoneNumber = await this.repository.AllNotDeletedReadonly<Manager>()
                .AnyAsync(m => m.User.PhoneNumber == phoneNumber
                && m.IsApproved == true
                && m.IsRejected == false);

            return existsByPhoneNumber;
        }

        public IEnumerable<string> GetAllManagerTypes()
        {
            IEnumerable<string> managerTypes =
                Enum.GetValues(typeof(ManagerType)).Cast<ManagerType>()
                .Select(mt => mt.ToString())
                .ToImmutableArray();

            return managerTypes;
        }
    }
}