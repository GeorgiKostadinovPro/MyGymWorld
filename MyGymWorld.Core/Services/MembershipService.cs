namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers.Memberships;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;

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
