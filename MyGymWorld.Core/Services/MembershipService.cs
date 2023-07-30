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
	using MyGymWorld.Core.Utilities.Contracts;

	public class MembershipService : IMembershipService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IQRCoderService qRCoderService;

        public MembershipService(
            IMapper _mapper, 
            IRepository _repository,
            IQRCoderService _qRCoderService)
        {
            this.mapper = _mapper;
            this.repository = _repository;

            this.qRCoderService = _qRCoderService;
        }

        public async Task<Membership> CreateMembershipAsync(CreateMembershipInputModel createMembershipInputModel)
        {
            Membership membership = this.mapper.Map<Membership>(createMembershipInputModel);

            membership.CreatedOn = DateTime.UtcNow;

            await this.repository.AddAsync(membership);
            await this.repository.SaveChangesAsync();

            return membership;
        }

		public async Task<Membership> EditMembershipAsync(string membershipId, EditMembershipInputModel editMembershipInputModel)
		{
            Membership membershipToEdit = await this.repository
                .All<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
                .FirstAsync();

            membershipToEdit.Price = editMembershipInputModel.Price;
            membershipToEdit.MembershipType = Enum.Parse<MembershipType>(editMembershipInputModel.MembershipType);
            membershipToEdit.ModifiedOn = DateTime.UtcNow;

            await this.repository.SaveChangesAsync();

            return membershipToEdit;
		}

        public async Task<Membership> DeleteMembershipAsync(string membershipId)
        {
            Membership membershipToEdit = await this.repository
               .All<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
               .Include(m => m.UsersMemberships)
               .FirstAsync();

            membershipToEdit.IsDeleted = true;
            membershipToEdit.DeletedOn = DateTime.UtcNow;

            IEnumerable<UserMembership> userMembershipsToDelete = membershipToEdit.UsersMemberships
                .Where(um => um.IsDeleted == false && um.MembershipId == Guid.Parse(membershipId))
                .ToArray();

            foreach (UserMembership userMembership in userMembershipsToDelete)
            {
                userMembership.IsDeleted = true;
                userMembership.DeletedOn = DateTime.UtcNow;
            }

            await this.repository.SaveChangesAsync();

            return membershipToEdit;
        }

        public async Task BuyMembershipAsync(string membershipId, string userId)
        {
            UserMembership? userMembership = await this.repository
                .All<UserMembership>(um => um.MembershipId == Guid.Parse(membershipId) && um.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            Membership membership = await this.repository
                .AllReadonly<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
                .FirstAsync();

            DateTime validTo = DateTime.UtcNow;

            if ((int)membership.MembershipType == 0)
            {
                validTo = DateTime.UtcNow.AddDays(7);
            }
            else if ((int)membership.MembershipType == 1)
            {
                validTo = DateTime.UtcNow.AddMonths(1);
            }
            else
            {
                validTo = DateTime.UtcNow.AddYears(1);
            }

            (string qRCodeUri, string publicId) = await this.qRCoderService.GenerateQRCodeAsync(membershipId);

            if (userMembership == null)
            {
                userMembership = new UserMembership
                {
                    UserId = Guid.Parse(userId),
                    MembershipId = Guid.Parse(membershipId),
                    ValidTo = validTo,
                    QRCodeUri = qRCodeUri,
                    PublicId = publicId,
                    CreatedOn = DateTime.UtcNow
                };

                await this.repository.AddAsync(userMembership);
            }
            else
            {
                if (userMembership.IsDeleted == true)
                {
                    userMembership.IsDeleted = false;
                    userMembership.DeletedOn = null;
                }

                userMembership.ValidTo = validTo;
                userMembership.QRCodeUri = qRCodeUri;
                userMembership.PublicId = publicId;
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<MembershipViewModel>> GetAllActiveUserMembershipsFilteredAndPagedByUserIdAsync(string userId, AllUserMemberhipsQueryModel queryModel)
        {
            IQueryable<Membership> membershipsAsQuery =
               this.repository.AllReadonly<UserMembership>(um => um.IsDeleted == false && um.UserId == Guid.Parse(userId))
                              .Include(um => um.Membership)
                                  .ThenInclude(m => m.Gym)
                                  .ThenInclude(g => g.Manager)
                                  .ThenInclude(ma => ma.User)
                              .Select(um => um.Membership);

            if (!string.IsNullOrWhiteSpace(queryModel.MembershipType))
            {
                membershipsAsQuery = membershipsAsQuery
                    .Where(m => m.MembershipType == Enum.Parse<MembershipType>(queryModel.MembershipType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                membershipsAsQuery = membershipsAsQuery
                  .Where(m => EF.Functions.Like(m.MembershipType.ToString(), wildCard));
            }

            switch (queryModel.Sorting)
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

            IEnumerable<MembershipViewModel> membershipsToDisplay
                = await membershipsAsQuery
                               .Skip((queryModel.CurrentPage - 1) * queryModel.MembershipsPerPage)
                               .Take(queryModel.MembershipsPerPage)
                               .ProjectTo<MembershipViewModel>(this.mapper.ConfigurationProvider)
                               .ToArrayAsync();

            return membershipsToDisplay;
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

            switch (queryModel.Sorting)
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
        
        public async Task<int> GetAllUserMembershipsCountByUserIdAsync(string userId)
        {
            return await this.repository.AllReadonly<UserMembership>(m => m.IsDeleted == false && m.UserId == Guid.Parse(userId))
                .CountAsync();
        }

        public async Task<MembershipDetailsViewModel> GetMembershipDetailsByIdAsync(string membershipId)
        {
            Membership membership = await this.repository
                .AllReadonly<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
                .Include(m => m.Gym)
                .FirstAsync();

            MembershipDetailsViewModel membershipDetailsViewModel = this.mapper.Map<MembershipDetailsViewModel>(membership);

            return membershipDetailsViewModel;
        } 

        public async Task<UserMembership?> GetUserMembershipAsync(string userId, string membershipId)
        {
            return await this.repository.AllReadonly<UserMembership>(um => um.IsDeleted == false)
                .FirstOrDefaultAsync(um => um.UserId == Guid.Parse(userId) && um.MembershipId == Guid.Parse(membershipId));
        }
        
        public async Task<EditMembershipInputModel> GetMembershipForEditByIdAsync(string membershipId)
		{
            Membership membershipToEdit = await this.repository
                .AllReadonly<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
                .FirstAsync();

            EditMembershipInputModel editMembershipInputModel = this.mapper.Map<EditMembershipInputModel>(membershipToEdit);

            return editMembershipInputModel;
		}

        public async Task<Membership?> GetMembershipByIdAsync(string membershipId)
        {
            return await this.repository.AllReadonly<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
                .FirstOrDefaultAsync();
        }
        
        public async Task<bool> CheckIfMembershipExistsByIdAsync(string membershipId)
		{
            return await this.repository
                .AllReadonly<Membership>(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId))
                .AnyAsync();
		}

        public async Task<int> GetAllActiveMembershipsCountAsync()
        {
            return await this.repository.AllReadonly<Membership>(m => m.IsDeleted == false)
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
