namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

    public class GymService : IGymService
    {
        private readonly IRepository repository;

        private readonly IAddressService addressService;

        public GymService(
            IRepository _repository, 
            IAddressService _addressService)
        {
            this.repository = _repository;
            this.addressService = _addressService;
        }

        public async Task CreateGymAsync(string managerId, CreateGymInputModel createGymInputModel)
        {
            Gym gym = new Gym
            {
                ManagerId = Guid.Parse(managerId),
                Name = createGymInputModel.Name,
                Email = createGymInputModel.Email,
                PhoneNumber = createGymInputModel.PhoneNumber,
                Description = createGymInputModel.Description,
                LogoUri = createGymInputModel.LogoParams!.SecureUri!.AbsoluteUri,
                LogoPublicId = createGymInputModel.LogoParams.PublicId,
                WebsiteUrl = createGymInputModel.WebsiteUrl,
                GymType = Enum.Parse<GymType>(createGymInputModel.GymType)
            };

            foreach (var galleryImageResultParams in createGymInputModel.GalleryImagesParams)
            {
                string uri = galleryImageResultParams.SecureUri.AbsoluteUri;
                string publicId = galleryImageResultParams.PublicId;

                gym.GymImages.Add(new GymImage
                {
                    Uri = uri,
                    PublicId = publicId,
                    CreatedOn = DateTime.UtcNow
                });
            }

            Address address = await this.addressService.GetAddressByNameAsync(createGymInputModel.Address);

            if (address != null)
            {
                gym.AddressId = address.Id;
                gym.Address = address;
            }
            else
            {
                Address createdAddress = await this.addressService.CreateAddressAsync(createGymInputModel.Address, createGymInputModel.TownId);

                gym.Address = createdAddress;
            }

            await this.repository.AddAsync(gym);
            await this.repository.SaveChangesAsync();
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
