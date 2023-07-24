namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Events;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class EventService : IEventService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        public EventService(IMapper _mapper, IRepository _repository)
        {
            this.mapper = _mapper;
            this.repository = _repository;
        }

        public async Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel)
        {
            IQueryable<Event> eventsAsQuery = this.repository.AllReadonly<Event>();

            return await eventsAsQuery
                .ProjectTo<EventViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }
    }
}
