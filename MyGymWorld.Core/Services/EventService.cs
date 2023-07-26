namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
	using MyGymWorld.Common;
	using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Events;
    using MyGymWorld.Web.ViewModels.Events.Enums;
    using MyGymWorld.Web.ViewModels.Managers.Events;
    using System;
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

        public async Task<Event> EditEventAsync(string eventId, EditEventInputModel editEventInputModel)
        {
            Event eventToEdit = await this.repository.All<Event>(e => e.IsDeleted == false && e.Id == Guid.Parse(eventId))
                .FirstAsync();

            eventToEdit.Name = editEventInputModel.Name;
            eventToEdit.Description = editEventInputModel.Description;
            eventToEdit.EventType = Enum.Parse<EventType>(editEventInputModel.EventType);
            eventToEdit.StartDate = editEventInputModel.ParsedStartDate;
            eventToEdit.EndDate = editEventInputModel.ParsedEndDate;
            eventToEdit.ModifiedOn = DateTime.UtcNow;

            await this.repository.SaveChangesAsync();

            return eventToEdit;
        }

		public async Task DeleteEventAsync(string eventId)
		{
            Event eventToDelete = await this.repository.All<Event>(e => e.IsDeleted == false && e.Id== Guid.Parse(eventId))
                .Include(e => e.UsersEvents)
                .FirstAsync();

            eventToDelete.IsDeleted = true;
            eventToDelete.DeletedOn = DateTime.UtcNow;

            foreach (UserEvent userEvent in eventToDelete.UsersEvents)
            {
                userEvent.IsDeleted = true;
                userEvent.DeletedOn = DateTime.UtcNow;
            }

            await this.repository.SaveChangesAsync();
		}

		public async Task ParticipateInEventAsync(string eventId, string userId)
        {
            UserEvent? userEvent = await this.repository
                .All<UserEvent>(ue => ue.EventId == Guid.Parse(eventId) && ue.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (userEvent == null)
            {
                userEvent = new UserEvent
                {
                    EventId = Guid.Parse(eventId),
                    UserId = Guid.Parse(userId),
                    CreatedOn = DateTime.UtcNow
                };

                await this.repository.AddAsync(userEvent);
            }
            else
            {
                if (userEvent.IsDeleted == true)
                {
                    userEvent.IsDeleted = false;
                    userEvent.DeletedOn = null;
                }
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task LeaveEventAsync(string eventId, string userId)
        {
			UserEvent? userEvent = await this.repository
			   .All<UserEvent>(ue => ue.EventId == Guid.Parse(eventId) && ue.UserId == Guid.Parse(userId))
			   .FirstOrDefaultAsync();

            if (userEvent == null)
            {
				throw new InvalidOperationException(ExceptionConstants.EventErrors.EventNotJoinedToBeLeft);
			}
            else
            {
                if (userEvent.IsDeleted == false)
                {
                    userEvent.IsDeleted = true;
                    userEvent.DeletedOn = DateTime.UtcNow;
                }
            }

            await this.repository.SaveChangesAsync();
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

        public async Task<IEnumerable<EventViewModel>> GetAllUserJoinedEventsFilteredAndPagedAsync(string userId, AllUserJoinedEventsQueryModel queryModel)
        {
            IQueryable<Event> eventsAsQuery =
               this.repository.AllReadonly<UserEvent>(ue => ue.IsDeleted == false && ue.UserId == Guid.Parse(userId))
                              .Include(ue => ue.Event)
                                  .ThenInclude(ue => ue.Gym)
                                  .ThenInclude(g => g.Manager)
                                  .ThenInclude(m => m.User)
                              .Select(ue => ue.Event);

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

        public async Task<int> GetAllUserJoinedEventsCountAsync(string userId)
        {
            return await this.repository.AllReadonly<UserEvent>(ue => ue.IsDeleted == false && ue.UserId == Guid.Parse(userId))
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

        public async Task<EditEventInputModel> GetEventForEditByIdAsync(string eventId)
        {
            Event eventToEdit = await this.repository.AllReadonly<Event>(e => e.IsDeleted == false && e.Id == Guid.Parse(eventId))
                .FirstAsync();

            EditEventInputModel editEventInputModel = this.mapper.Map<EditEventInputModel>(eventToEdit);

            return editEventInputModel;
        }

        public async Task<Event?> GetEventByIdAsync(string eventId)
        {
            return await this.repository.AllReadonly<Event>(e => e.IsDeleted == false && e.Id == Guid.Parse(eventId))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CheckIfEventExistsByIdAsync(string eventId)
        {
            return await this.repository.AllReadonly<Event>(e => e.IsDeleted == false && e.Id == Guid.Parse(eventId))
                .AnyAsync();
        }

        public async Task<bool> CheckIfUserHasAlreadyJoinedEventByIdAsync(string eventId, string userId)
        {
            return await this.repository.All<UserEvent>(ue => ue.IsDeleted == false)
                .AnyAsync(ue => ue.EventId == Guid.Parse(eventId) && ue.UserId == Guid.Parse(userId));
        }

        public async Task<bool> CheckIfUserHasAlreadyLeftEventByIdAsync(string eventId, string userId)
        {
            return await this.repository.All<UserEvent>(ue => ue.IsDeleted == true)
                .AnyAsync(ue => ue.EventId == Guid.Parse(eventId) && ue.UserId == Guid.Parse(userId));
        }

        public async Task<int> GetAllActiveEventsCountAsync()
        {
            var result =  await this.repository.AllReadonly<Event>(e => e.IsDeleted == false)
                .CountAsync();
            return result;
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