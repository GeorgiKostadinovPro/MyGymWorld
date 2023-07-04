namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Countries;

    public interface ICountryService
    {
        Task<IEnumerable<CountryViewModel>> GetAllAsync();

        Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();

        Task<Country> GetCountryByIdAsync(Guid countryId);
    }
}
