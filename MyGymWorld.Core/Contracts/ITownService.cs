namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Data.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITownService
    {
        Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();

        Task<Town?> GetTownByIdAsync(string townId);


        Task<bool> CheckIfTownIsPresentByCountryIdAsync(string townId, string countryId);
    }
}