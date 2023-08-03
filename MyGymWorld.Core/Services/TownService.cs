namespace MyGymWorld.Core.Services
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TownService : ITownService
    {
        private readonly IRepository repository;

        public TownService(IRepository _repository)
        {
            this.repository = _repository;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Town>()
                .Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Id.ToString()
                })
                .OrderBy(sli => sli.Text)
                .ToArrayAsync();
        }

        public async Task<Town?> GetTownByIdAsync(string townId)
        {
            Town? town = await this.repository.AllNotDeletedReadonly<Town>()
                .FirstOrDefaultAsync(t => t.Id.ToString() == townId);

            return town;
        }

        public async Task<bool> CheckIfTownIsPresentByCountryIdAsync(string townId, string countryId)
        {
            Town? town = await this.repository.AllNotDeletedReadonly<Town>()
               .FirstOrDefaultAsync(t => t.Id.ToString() == townId && t.CountryId.ToString() == countryId);

            return town != null ? true : false;
        }
    }
}