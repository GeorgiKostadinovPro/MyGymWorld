namespace MyGymWorld.Core.Contracts
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Towns;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface ITownService
    {
        Task<IEnumerable<TownViewModel>> GetAllAsync();

        Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync();

        Task<Town?> GetTownByIdAsync(Guid townId);


        Task<bool> CheckIfTownIsPresentByCountryIdAsync(string townId, string countryId);
    }
}