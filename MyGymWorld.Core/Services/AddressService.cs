namespace MyGymWorld.Core.Services
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System;

    public class AddressService : IAddressService
    {
        private readonly IRepository repository;

        public AddressService(IRepository _repository)
        {
            this.repository = _repository;
        }
        
        public async Task<Address> CreateAddressAsync(string name, string townId)
        {
            Address address = new Address
            {
                Name = name,
                TownId = Guid.Parse(townId),
                CreatedOn = DateTime.UtcNow
            };

            await this.repository.AddAsync(address);
            await this.repository.SaveChangesAsync();

            return address;
        }
        
        public async Task<Address?> GetAddressByIdAsync(string addressId)
        {
            Address? address = await this.repository.AllNotDeletedReadonly<Address>()
                .FirstOrDefaultAsync(a => a.Id.ToString() == addressId);

            return address;
        }

        public async Task<Address?> GetAddressByNameAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                return null;
            }

            string wildCard = $"%{address.ToLower()}%";

            Address? addressToGet = await this.repository.AllNotDeleted<Address>()
                .FirstOrDefaultAsync(a => EF.Functions.Like(a.Name, wildCard));

            return addressToGet;
        } 
    }
}
