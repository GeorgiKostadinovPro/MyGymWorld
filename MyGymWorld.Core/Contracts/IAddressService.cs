namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;

    public interface IAddressService
    {
        Task<Address> CreateAddressAsync(string name, string townId);

        Task<Address?> GetAddressByIdAsync(Guid addressId);

        Task<Address?> GetAddressByNameAsync(string address);
    }
}
