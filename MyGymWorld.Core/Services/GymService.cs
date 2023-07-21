namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using MyGymWorld.Web.ViewModels.Gyms;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using MyGymWorld.Web.ViewModels.Gyms.Enums;
    using MyGymWorld.Common;

    public class GymService : IGymService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IUserService userService;
        private readonly IAddressService addressService;

        public GymService(
            IMapper _mapper,
            IRepository _repository,
            IUserService _userService,
            IAddressService _addressService)
        {
            this.mapper = _mapper;
            this.repository = _repository;

            this.userService = _userService;
            this.addressService = _addressService;
        }

        public async Task CreateGymAsync(Guid managerId, CreateGymInputModel createGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel)
        {
            Gym gym = new Gym
            {
                ManagerId = managerId,
                Name = createGymInputModel.Name,
                Email = createGymInputModel.Email,
                PhoneNumber = createGymInputModel.PhoneNumber,
                Description = createGymInputModel.Description,
                LogoUri = gymLogoAndGalleryImagesInputModel.LogoResultParams!.SecureUri!.AbsoluteUri,
                LogoPublicId = gymLogoAndGalleryImagesInputModel.LogoResultParams.PublicId,
                WebsiteUrl = createGymInputModel.WebsiteUrl,
                GymType = Enum.Parse<GymType>(createGymInputModel.GymType),
                CreatedOn = DateTime.UtcNow
            };
            
            Address address = await this.addressService.GetAddressByNameAsync(createGymInputModel.Address);

            if (address != null)
            {
                gym.AddressId = address.Id;
            }
            else
            {
                Address createdAddress = await this.addressService.CreateAddressAsync(createGymInputModel.Address, createGymInputModel.TownId);

                gym.AddressId = createdAddress.Id;
            }  
            
            foreach (var galleryImageResultParams in gymLogoAndGalleryImagesInputModel.GalleryImagesResultParams)
            {
                string uri = galleryImageResultParams.SecureUri.AbsoluteUri;
                string publicId = galleryImageResultParams.PublicId;

                gym.GymImages.Add(new GymImage
                {
                    GymId = gym.Id,
                    Uri = uri,
                    PublicId = publicId,
                    CreatedOn = DateTime.UtcNow,
                });
            }

            await this.repository.AddAsync(gym);
            await this.repository.SaveChangesAsync();
        }

        public async Task EditGymAsync(string gymId, EditGymInputModel editGymInputModel, GymLogoAndGalleryImagesInputModel gymLogoAndGalleryImagesInputModel)
        {
            Gym gymToEdit = await this.repository.All<Gym>(g => g.IsDeleted == false && g.Id == Guid.Parse(gymId))
                .Include(g => g.Address)
                .ThenInclude(a => a.Town)
                .FirstOrDefaultAsync();

            gymToEdit.Name = editGymInputModel.Name;
            gymToEdit.Email = editGymInputModel.Email;
            gymToEdit.PhoneNumber = gymToEdit.PhoneNumber;
            gymToEdit.WebsiteUrl = editGymInputModel.WebsiteUrl;
            gymToEdit.Description = editGymInputModel.Description;
            gymToEdit.GymType = Enum.Parse<GymType>(editGymInputModel.GymType);

            Address address = await this.addressService.GetAddressByNameAsync(editGymInputModel.Address);

            if (address != null)
            {
                gymToEdit.AddressId = address.Id;
            }
            else
            {
                Address createdAddress = await this.addressService.CreateAddressAsync(editGymInputModel.Address, editGymInputModel.TownId);

                gymToEdit.AddressId = createdAddress.Id;
            }

            if (gymLogoAndGalleryImagesInputModel.LogoResultParams != null)
            {
                gymToEdit.LogoUri = gymLogoAndGalleryImagesInputModel.LogoResultParams!.SecureUri!.AbsoluteUri;
                gymToEdit.LogoPublicId = gymLogoAndGalleryImagesInputModel.LogoResultParams.PublicId;
            }

            if (gymLogoAndGalleryImagesInputModel.GalleryImagesResultParams.Count > 0)
            {
                foreach (var galleryImageResultParams in gymLogoAndGalleryImagesInputModel.GalleryImagesResultParams)
                {
                    string uri = galleryImageResultParams.SecureUri.AbsoluteUri;
                    string publicId = galleryImageResultParams.PublicId;

                    gymToEdit.GymImages.Add(new GymImage
                    {
                        GymId = gymToEdit.Id,
                        Uri = uri,
                        PublicId = publicId,
                        CreatedOn = DateTime.UtcNow,
                    });
                }
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(Guid managerId, bool isDeleted, int skip = 0, int? take = null)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == isDeleted && g.ManagerId == managerId)
                .OrderByDescending(g => g.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                gymsAsQuery = gymsAsQuery.Take(take.Value);
            }

            return await gymsAsQuery
                .ProjectTo<GymViewModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        public async Task<List<GymViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == isDeleted)
                .OrderByDescending(g => g.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                gymsAsQuery = gymsAsQuery.Take(take.Value);
            }

            return await gymsAsQuery
               .ProjectTo<GymViewModel>(this.mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<int> GetActiveOrDeletedGymsCountForManagementAsync(Guid managerId, bool isDeleted)
        {
            return await this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == isDeleted && g.ManagerId == managerId)
                .CountAsync();
        }

        public async Task<int> GetActiveOrDeletedGymsCountForAdministrationAsync(bool isDeleted)
        {
            return await this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == isDeleted)
                .CountAsync();
        }

        public async Task<int> GetActiveGymsCountAsync()
        {
            return await this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == false)
                .CountAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetTop10NewestActiveGymsAsync()
        {
            return await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false)
                .OrderByDescending(g => g.CreatedOn)
                .Take(10)
                .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetTop10MostLikedActiveGymsAsync()
        {
            return await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false)
                .OrderByDescending(g => g.Likes.Count)
                .Take(10)
                .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetAllFilteredAndPagedActiveGymsAsync(AllGymsQueryModel queryModel)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == false)
                .Include(g => g.Address);

            if (!string.IsNullOrWhiteSpace(queryModel.GymType))
            {
                gymsAsQuery = gymsAsQuery
                    .Where(g => g.GymType == Enum.Parse<GymType>(queryModel.GymType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                gymsAsQuery = gymsAsQuery
                    .Where(g => EF.Functions.Like(g.Name, wildCard)
                    || EF.Functions.Like(g.Description, wildCard)
                    || EF.Functions.Like(g.Address.Name, wildCard));
            }

            switch (queryModel.GymsSorting)
            {
                case GymsSorting.Newest:
                    gymsAsQuery = gymsAsQuery
                        .OrderByDescending(g => g.CreatedOn);
                    break;
                case GymsSorting.Oldest:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.CreatedOn);
                    break;
                case GymsSorting.LikesAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Likes.Count);
                    break;
                case GymsSorting.LikesDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Likes.Count);
                    break;
                case GymsSorting.CommentsAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Comments.Count);
                    break;
                case GymsSorting.CommentsDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Comments.Count);
                    break;
                default:
                    break;
            }

            IEnumerable<DisplayGymViewModel> gymsToDisplay
                = await gymsAsQuery
                             .Skip((queryModel.CurrentPage - 1) * queryModel.GymsPerPage)
                             .Take(queryModel.GymsPerPage)
                             .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                             .ToArrayAsync();

            return gymsToDisplay;
        }

        public async Task<GymDetailsViewModel> GetGymDetailsByIdAsync(string gymId)
        {
            Gym? gym = await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false)
                .Include(g => g.Manager)
                   .ThenInclude(m => m.User)
                .Include(g => g.Address)
                   .ThenInclude(a => a.Town)
                      .ThenInclude(t => t.Country)
                .Include(g => g.GymImages)
                .FirstOrDefaultAsync(g => g.Id == Guid.Parse(gymId));

            GymDetailsViewModel gymDetailsViewModel = this.mapper.Map<GymDetailsViewModel>(gym);

            return gymDetailsViewModel;
        }

        public async Task<EditGymInputModel> GetGymForEditByIdAsync(string gymId)
		{
            Gym? gymForEdit = await this.repository.AllReadonly<Gym>(g => g.Id == Guid.Parse(gymId) && g.IsDeleted == false)
                .Include(g => g.Address)
                .ThenInclude(a => a.Town)
				.FirstOrDefaultAsync();

            EditGymInputModel editGymInputModel = new EditGymInputModel
            {
                Id = gymForEdit!.Id.ToString(),
                Name = gymForEdit.Name,
                Email = gymForEdit.Email,
                PhoneNumber = gymForEdit.PhoneNumber,
                WebsiteUrl = gymForEdit.WebsiteUrl,
                Description = gymForEdit.Description,
                GymType = gymForEdit.GymType.ToString(),
                Address = gymForEdit.Address.Name,
                TownId = gymForEdit.Address.TownId.ToString(),
                CountryId = gymForEdit.Address.Town.CountryId.ToString()
            };

            return editGymInputModel;
		} 
        
        public async Task AddGymToUserAsync(string gymId, string userId)
        {
            Gym gym = await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false)
                .FirstAsync(g => g.Id == Guid.Parse(gymId));

            ApplicationUser user = await this.userService.GetUserByIdAsync(userId);

            UserGym userGym = await this.repository.AllReadonly<UserGym>()
                .FirstOrDefaultAsync(ug => ug.GymId == Guid.Parse(gymId) && ug.UserId == Guid.Parse(userId));

            if (userGym != null)
            {
                if (userGym.IsDeleted == false)
                {
                    throw new InvalidOperationException(ExceptionConstants.GymErrors.GymAlreadyJoined);
                }
                else
                {
                    userGym.IsDeleted = false;
                    userGym.DeletedOn = null;
                    userGym.ModifiedOn = DateTime.UtcNow;
                }            
            }
            else
            {
                userGym = new UserGym 
                { 
                    GymId = gym.Id,
                    UserId = user.Id,
                    CreatedOn = DateTime.UtcNow
                }; 
                
                await this.repository.AddAsync(userGym);
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task RemoveGymFromUserAsync(string gymId, string userId)
        {
            UserGym userGym = await this.repository.AllReadonly<UserGym>()
                .FirstAsync(ug => ug.Id == Guid.Parse(gymId) && ug.UserId == Guid.Parse(userId));

            if (userGym == null)
            {
                throw new InvalidOperationException(ExceptionConstants.GymErrors.GymNotJoinedToBeLeft);
            }
            else
            {
                if (userGym.IsDeleted == true)
                {
                    throw new InvalidOperationException(ExceptionConstants.GymErrors.GymAlreadyLeft);
                }
                else
                {
                    userGym.IsDeleted = true;
                    userGym.DeletedOn = DateTime.UtcNow;
                }
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetAllUserJoinedGymsAsync(string userId, AllGymsQueryModel queryModel)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllReadonly<UserGym>(ug => ug.IsDeleted == false && ug.UserId == Guid.Parse(userId))
                .Include(ug => ug.Gym)
                .ThenInclude(g => g.Address)
                .Select(ug => ug.Gym);

            if (!string.IsNullOrWhiteSpace(queryModel.GymType))
            {
                gymsAsQuery = gymsAsQuery
                    .Where(g => g.GymType == Enum.Parse<GymType>(queryModel.GymType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                gymsAsQuery = gymsAsQuery
                    .Where(g => EF.Functions.Like(g.Name, wildCard)
                    || EF.Functions.Like(g.Description, wildCard)
                    || EF.Functions.Like(g.Address.Name, wildCard));
            }

            switch (queryModel.GymsSorting)
            {
                case GymsSorting.Newest:
                    gymsAsQuery = gymsAsQuery
                        .OrderByDescending(g => g.CreatedOn);
                    break;
                case GymsSorting.Oldest:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.CreatedOn);
                    break;
                case GymsSorting.LikesAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Likes.Count);
                    break;
                case GymsSorting.LikesDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Likes.Count);
                    break;
                case GymsSorting.CommentsAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Comments.Count);
                    break;
                case GymsSorting.CommentsDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Comments.Count);
                    break;
                default:
                    break;
            }

            IEnumerable<DisplayGymViewModel> gymsToDisplay
                = await gymsAsQuery
                             .Skip((queryModel.CurrentPage - 1) * queryModel.GymsPerPage)
                             .Take(queryModel.GymsPerPage)
                             .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                             .ToArrayAsync();

            return gymsToDisplay;
        }

        public async Task<int> GetAllUserJoinedGymsCountAsync(string userId)
        {
            return await this.repository.AllReadonly<UserGym>(ug => ug.IsDeleted == false && ug.UserId == Guid.Parse(userId))
                .CountAsync();
        }

        public async Task<bool> CheckIfGymExistsByIdAsync(string gymId)
		{
            return await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false && g.Id == Guid.Parse(gymId))
                .AnyAsync();
		}

        public async Task<bool> CheckIfGymIsManagedByManagerAsync(string gymId, string mananerId)
        {
            return await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false)
                .AnyAsync(g => g.Id == Guid.Parse(gymId) && g.ManagerId == Guid.Parse(mananerId));
        }

        public async Task<Gym> GetGymByIdAsync(string gymId)
        {
            return await this.repository.AllReadonly<Gym>(g => g.IsDeleted == false)
                .FirstAsync(g => g.Id == Guid.Parse(gymId));
        }

        public IEnumerable<string> GetAllGymTypes()
        {
            IEnumerable<string> gymTypes = Enum.GetValues(typeof(GymType))
                .Cast<GymType>()
                .Select(gt => gt.ToString())
                .ToImmutableArray();

            return gymTypes;    
        }
    }
}