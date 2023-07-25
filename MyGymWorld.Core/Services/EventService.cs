namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Events.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Events;
    using System.Collections.Generic;
    using System.Collections.Immutable;
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

        public async Task<Event> CreateEventAsync(CreateEventInputModel createEventInputModel)
        {
            Event @event = this.mapper.Map<Event>(createEventInputModel);

            @event.CreatedOn = DateTime.UtcNow;

            await this.repository.AddAsync(@event);
            await this.repository.SaveChangesAsync();

            return @event;
        }

        public async Task<IEnumerable<EventViewModel>> GetAllActiveEventsFilteredAndPagedByGymIdAsync(AllEventsForGymQueryModel queryModel)
        {
            IQueryable<Event> eventsAsQuery =
                this.repository.AllReadonly<Event>(e => e.IsDeleted == false && e.GymId == Guid.Parse(queryModel.GymId))
                               .Include(e => e.UsersEvents)
                               .Include(e => e.Gym)
                               .ThenInclude(g => g.Manager)
                               .ThenInclude(m => m.User);

            if (!string.IsNullOrWhiteSpace(queryModel.EventType))
            {
                eventsAsQuery = eventsAsQuery
                    .Where(e => e.EventType == Enum.Parse<EventType>(queryModel.EventType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                eventsAsQuery = eventsAsQuery
                    .Where(e => EF.Functions.Like(e.Name, wildCard))
                    .Where(e => EF.Functions.Like(e.Description, wildCard))
                    .Where(e => EF.Functions.Like(e.Gym.Name, wildCard));
            }

            switch (queryModel.EventsSorting)
            {
                case EventsSorting.Newest:
                    eventsAsQuery = eventsAsQuery
                        .OrderByDescending(e => e.CreatedOn);
                    break;
                case EventsSorting.Oldest:
                    eventsAsQuery = eventsAsQuery
                       .OrderBy(e => e.CreatedOn);
                    break;
                case EventsSorting.ParticipantsAscending:
                    eventsAsQuery = eventsAsQuery
                       .OrderBy(e => e.UsersEvents.Count);
                    break;
                case EventsSorting.ParticipantsDescending:
                    eventsAsQuery = eventsAsQuery
                      .OrderBy(e => e.UsersEvents.Count);
                    break;
            }

            IEnumerable<EventViewModel> eventsToDisplay
                = await eventsAsQuery
                               .Skip((queryModel.CurrentPage - 1) * queryModel.EventsPerPage)
                               .Take(queryModel.EventsPerPage)
                               .ProjectTo<EventViewModel>(this.mapper.ConfigurationProvider)
                               .ToArrayAsync();

            return eventsToDisplay;

        }
        
        public async Task<int> GetAllActiveEventsCountByGymIdAsync(string gymId)
        {
            return await this.repository.AllReadonly<Event>(e => e.IsDeleted == false && e.GymId == Guid.Parse(gymId))
                .CountAsync();
        }

        public async Task<EventDetailsViewModel> GetEventDetailsByIdAsync(string eventId)
        {
            Event eventToDisplay = await this.repository
                   .AllReadonly<Event>(e => e.IsDeleted == false && e.Id == Guid.Parse(eventId))
                   .Include(e => e.Gym)
                      .ThenInclude(g => g.Manager)
                      .ThenInclude(m => m.User)
                      .FirstAsync();

            EventDetailsViewModel eventDetailsViewModel = this.mapper.Map<EventDetailsViewModel>(eventToDisplay);

            return eventDetailsViewModel;
        }

        public async Task<bool> CheckIfEventExistsByIdAsync(string eventId)
        {
            return await this.repository.AllReadonly<Event>(e => e.IsDeleted == false && e.Id == Guid.Parse(eventId))
                .AnyAsync();
        } 

        public IEnumerable<string> GetAllEventTypes()
        {
            IEnumerable<string> eventTypes = Enum.GetValues(typeof(EventType))
              .Cast<EventType>()
              .Select(e => e.ToString())
              .ToImmutableArray();

            return eventTypes;
        }
    }
}