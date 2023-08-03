namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;

    public interface IAddressService
    {
        Task<Address> CreateAddressAsync(string name, string townId);

        Task<Address?> GetAddressByIdAsync(string addressId);

        Task<Address?> GetAddressByNameAsync(string address);
    }
}
