namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Countries;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class CountryService : ICountryService
    {
        private readonly IRepository repository;
        private readonly IMapper mapper;

        public CountryService(IRepository _repository, IMapper _mapper)
        {
            this.repository = _repository;
            this.mapper = _mapper;
        }

        public async Task<IEnumerable<CountryViewModel>> GetAllAsync()
        {
            return await this.repository.AllReadonly<Country>()
                .ProjectTo<CountryViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetAllAsSelectListItemsAsync()
        {
            return await this.repository.AllReadonly<Country>()
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
                .OrderBy(sli => sli.Text)
                .ToArrayAsync();
        }
    }
}