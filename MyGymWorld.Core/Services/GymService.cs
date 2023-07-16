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
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    public class GymService : IGymService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IAddressService addressService;

        public GymService(
            IMapper _mapper,
            IRepository _repository,
            IAddressService _addressService)
        {
            this.mapper = _mapper;
            this.repository = _repository;

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

        public async Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(bool isDeleted, int skip = 0, int? take = null)
        {
            IQueryable<Gym> gymsAsQuery = this.repository.AllReadonly<Gym>(g => g.IsDeleted == isDeleted)
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

        public async Task<int> GetActiveOrDeletedGymsCountAsync(bool isDeleted)
        {
            return await this.repository
                .AllReadonly<Gym>(g => g.IsDeleted == isDeleted)
                .CountAsync();
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
