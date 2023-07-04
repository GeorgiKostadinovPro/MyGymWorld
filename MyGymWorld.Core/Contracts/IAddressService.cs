namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;

    public interface IAddressService
    { 
        Task<Address> GetAddressByIdAsync(Guid addressId);

        Task<Address> GetAddressByNameAsync(string address);
    }
}
