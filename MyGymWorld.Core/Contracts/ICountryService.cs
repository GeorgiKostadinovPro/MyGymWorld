namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Data.Models;

    public interface ICountryService
    {
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();

        Task<Country?> GetCountryByIdAsync(string countryId);
    }
}
