namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Web.ViewModels.Towns;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITownService
    {
        Task<IEnumerable<TownViewModel>> GetAllAsync();

        Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();
        
        Task<bool> CheckIfTownIsPresentByCountryIdAsync(string townId, string countryId);
    }
}