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

        public async Task<Address> GetAddressByNameAsync(string address)
        {
            Address addressToGet = await this.repository.All<Address>()
                .FirstOrDefaultAsync(a => a.Name.ToLower().Contains(address.ToLower()));

            return addressToGet;
        } 
        
        public async Task<Address> GetAddressByIdAsync(Guid addressId)
        {
            Address address = await this.repository.All<Address>()
                .FirstOrDefaultAsync(a => a.Id == addressId);

            return address;
        }
    }
}
