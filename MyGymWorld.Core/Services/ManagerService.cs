namespace MyGymWorld.Core.Services
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
            bool isManagerTypeValid = Enum.TryParse<ManagerType>(becomeManagerInputModel.ManagerType, true, out ManagerType managerType);

            if (!isManagerTypeValid)
            {
                throw new InvalidOperationException(ExceptionConstants.ManagerErrors.InvalidManagerType);
            }

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
                ManagerType = managerType,
                CreatedOn = DateTime.UtcNow,
                IsApproved = false
            };
            
            await this.repository.AddAsync(manager);
            await this.repository.SaveChangesAsync();

            ApplicationUser admin = await this.userService.GetAdministratorAsync();

            await this.notificationService.CreateNotificationAsync(
                "You have succesfully applied for Manager! Admin will approve you soon!", 
                null!, 
                manager.UserId.ToString());

            await this.notificationService.CreateNotificationAsync(
                $"You have received a manager request from {user.FirstName} {user.LastName}",
                $"/Administration/Manager/Requests",
                admin.Id.ToString());
        }

        public async Task<Manager> ApproveManagerAsync(string managerId, string adminId)
        {
            Manager manager = await this.repository.All<Manager>(m => m.IsDeleted == false)
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse(managerId));

            if (manager == null)
            {
                throw new InvalidOperationException(string.Format(ExceptionConstants.ManagerErrors.InvalidManagerId, managerId));
            }

            manager.IsApproved = true;
            manager.ModifiedOn = DateTime.UtcNow;

            await this.repository.SaveChangesAsync();

            await this.roleService.AddRoleToUserAsync(manager.UserId.ToString(), "Administrator");

            await this.notificationService.CreateNotificationAsync(
                "Your request was approved! You are now a manager!",
                "/User/UserProfile",
                managerId);

            await this.notificationService.CreateNotificationAsync(
               $"You approved {manager.User.FirstName} {manager.User.LastName}!",
               "Administration/Manager/Requests",
               adminId);

            return manager;
        }

        public async Task<IEnumerable<ManagerRequestViewModel>> GetAllNotApprovedManagerRequestsAsync()
        {
            return await this.repository.AllReadonly<Manager>(m => m.IsDeleted == false && m.IsApproved == false)
                .ProjectTo<ManagerRequestViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<BecomeManagerInputModel> GetUserToBecomeManagerByIdAsync(string userId)
        {
            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            BecomeManagerInputModel becomeManagerInputModel = this.mapper.Map<BecomeManagerInputModel>(user);
            becomeManagerInputModel.IsApproved = false;
            becomeManagerInputModel.ManagerTypes = this.GetAllManagerTypes();

            return becomeManagerInputModel;
        }

        public IEnumerable<string> GetAllManagerTypes()
        {
            IEnumerable<string> managerTypes =
                Enum.GetValues(typeof(ManagerType)).Cast<ManagerType>()
                .ToImmutableArray()
                .Select(mt => mt.ToString());

            return managerTypes;
        }

        public async Task<bool> CheckIfUserIsAManagerAsync(string userId)
        {
            bool isManager = await this.repository.AllReadonly<Manager>(m => m.IsDeleted == false)
                .AnyAsync(m => m.UserId == Guid.Parse(userId));

            return isManager;   
        }

        public async Task<bool> CheckIfManagerExistsByPhoneNumberAsync(string phoneNumber)
        {
            bool existsByPhoneNumber = await this.repository.AllReadonly<ApplicationUser>(m => m.IsDeleted == false)
                .AnyAsync(u => u.PhoneNumber == phoneNumber);

            return existsByPhoneNumber;
        }
    }
}
