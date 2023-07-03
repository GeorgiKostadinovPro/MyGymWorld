namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Towns;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class TownService : ITownService
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        public TownService(IRepository _repository, IMapper _mapper)
        {
            this.repository = _repository;
            this.mapper = _mapper;
        }

        public async Task<IEnumerable<TownViewModel>> GetAllAsync()
        {
            return await this.repository.AllReadonly<Town>()
               .ProjectTo<TownViewModel>(this.mapper.ConfigurationProvider)
               .ToArrayAsync();
        } 
        
        public async Task<bool> CheckIfTownIsPresentByCountryIdAsync(string townId, string countryId)
        {
            Town town = await this.repository.AllReadonly<Town>()
               .FirstOrDefaultAsync(t => t.Id.ToString() == townId && t.CountryId.ToString() == countryId);

            return town != null ? true : false;
        }
        
        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync()
        {
            return await this.repository.AllReadonly<Town>()
                .Select(t => new SelectListItem
                {
                    Text = t.Name,
                    Value = t.Id.ToString()
                })
                .OrderBy(sli => sli.Text)
                .ToArrayAsync();
        }
    }
}