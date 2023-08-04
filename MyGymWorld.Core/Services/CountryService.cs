namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CountryService : ICountryService
    {
        private readonly IRepository repository;
        public CountryService(IRepository _repository)
        {
            this.repository = _repository;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Country>()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .OrderBy(sli => sli.Text)
                .ToArrayAsync();
        }

        public async Task<Country?> GetCountryByIdAsync(string countryId)
        {
            Country? country = await this.repository.AllNotDeleted<Country>()
                .FirstOrDefaultAsync(c => c.Id.ToString() == countryId);

            return country;
        }
    }
}