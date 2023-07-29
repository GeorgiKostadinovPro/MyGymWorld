namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers.Memberships;
    using MyGymWorld.Web.ViewModels.Memberships;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using MyGymWorld.Web.ViewModels.Memberships.Enums;
    using AutoMapper.QueryableExtensions;

    public class MembershipService : IMembershipService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        public MembershipService(
            IMapper _mapper, 
            IRepository _repository)
        {
            this.mapper = _mapper;
            this.repository = _repository;
        }

        public async Task<Membership> CreateMembershipAsync(CreateMembershipInputModel createMembershipInputModel)
        {
            Membership membership = this.mapper.Map<Membership>(createMembershipInputModel);

            membership.CreatedOn = DateTime.UtcNow;

            await this.repository.AddAsync(membership);
            await this.repository.SaveChangesAsync();

            return membership;
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllActiveMembershipsFilteredAndPagedByGymIdAsync(AllMembershipsForGymQueryModel queryModel)
        {
            IQueryable<Membership> membershipsAsQuery =
                this.repository.AllReadonly<Membership>(e => e.IsDeleted == false && e.GymId == Guid.Parse(queryModel.GymId))
                               .Include(e => e.Gym);

            if (!string.IsNullOrWhiteSpace(queryModel.MembershipType))
            {
                membershipsAsQuery = membershipsAsQuery
                    .Where(e => e.MembershipType == Enum.Parse<MembershipType>(queryModel.MembershipType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                membershipsAsQuery = membershipsAsQuery
                    .Where(e => EF.Functions.Like(e.Price.ToString(), wildCard)
                    || EF.Functions.Like(e.MembershipType.ToString(), wildCard)
                    || EF.Functions.Like(e.Gym.Name, wildCard));
            }

            switch (queryModel.MembershipsSorting)
            {
                case MembershipsSorting.Newest:
                    membershipsAsQuery = membershipsAsQuery
                        .OrderByDescending(m => m.CreatedOn);
                    break;
                case MembershipsSorting.Oldest:
                    membershipsAsQuery = membershipsAsQuery
                       .OrderBy(m => m.CreatedOn);
                    break;
                case MembershipsSorting.PriceAscending:
                    membershipsAsQuery = membershipsAsQuery
                       .OrderBy(m => m.Price);
                    break;
                case MembershipsSorting.PriceDescending:
                    membershipsAsQuery = membershipsAsQuery
                      .OrderBy(m => m.Price);
                    break;
            }

            IEnumerable<MembershipViewModel> eventsToDisplay
                = await membershipsAsQuery
                               .Skip((queryModel.CurrentPage - 1) * queryModel.MembershipsPerPage)
                               .Take(queryModel.MembershipsPerPage)
                               .ProjectTo<MembershipViewModel>(this.mapper.ConfigurationProvider)
                               .ToArrayAsync();

            return eventsToDisplay;
        }
        
        public async Task<int> GetAllActiveMembershipsCountByGymIdAsync(string gymId)
        {
            return await this.repository.AllReadonly<Membership>(m => m.IsDeleted == false && m.GymId == Guid.Parse(gymId))
                .CountAsync();
        }

        public IEnumerable<string> GetAllMembershipTypes()
        {
            IEnumerable<string> membershipTypes = Enum.GetValues(typeof(MembershipType))
              .Cast<MembershipType>()
              .Select(e => e.ToString())
              .ToImmutableArray();

            return membershipTypes;
        }
    }
}
