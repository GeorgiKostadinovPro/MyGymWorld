namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Common;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    public class ManagerService : IManagerService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IUserService userService;

        
        public ManagerService(IMapper _mapper, IRepository repository, IUserService _userService)
        {
            this.mapper = _mapper;
            this.repository = repository;

            this.userService = _userService;
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
                ManagerType = managerType
            };

            await this.repository.AddAsync(manager);
            await this.repository.SaveChangesAsync();
        }

        public async Task<BecomeManagerInputModel> GetUserToBecomeManagerByIdAsync(string userId)
        {
            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            BecomeManagerInputModel becomeManagerInputModel = this.mapper.Map<BecomeManagerInputModel>(user);
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
            bool isManager = await this.repository.AllReadonly<Manager>()
                .AnyAsync(m => m.UserId == Guid.Parse(userId));

            return isManager;   
        }

        public async Task<bool> CheckIfManagerExistsNyPhoneNumberAsync(string phoneNumber)
        {
            bool existsByPhoneNumber = await this.repository.AllReadonly<ApplicationUser>()
                .AnyAsync(u => u.PhoneNumber == phoneNumber);

            return existsByPhoneNumber;
        }
    }
}
