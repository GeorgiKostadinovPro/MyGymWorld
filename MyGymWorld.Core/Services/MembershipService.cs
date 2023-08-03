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

		public async Task EditMembershipAsync(string membershipId, EditMembershipInputModel editMembershipInputModel)
		{
            Membership? membershipToEdit = await this.repository
                .AllNotDeleted<Membership>()
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse(membershipId));

            if (membershipToEdit != null)
            {
                membershipToEdit.Price = editMembershipInputModel.Price;
                membershipToEdit.MembershipType = Enum.Parse<MembershipType>(editMembershipInputModel.MembershipType);
                membershipToEdit.ModifiedOn = DateTime.UtcNow;

                await this.repository.SaveChangesAsync();
            }
		}

        public async Task DeleteMembershipAsync(string membershipId)
        {
            Membership? membershipToDelete = await this.repository
               .AllNotDeleted<Membership>()
               .Where(m => m.Id == Guid.Parse(membershipId))
               .Include(m => m.UsersMemberships)
               .FirstOrDefaultAsync();

            if (membershipToDelete != null)
            {
                membershipToDelete.IsDeleted = true;
                membershipToDelete.DeletedOn = DateTime.UtcNow;

                IEnumerable<UserMembership> userMembershipsToDelete = membershipToDelete.UsersMemberships
                    .Where(um => um.IsDeleted == false)
                    .ToArray();

                foreach (UserMembership userMembership in userMembershipsToDelete)
                {
                    userMembership.IsDeleted = true;
                    userMembership.DeletedOn = DateTime.UtcNow;
                }

                await this.repository.SaveChangesAsync();
            }
        }

        public async Task BuyMembershipAsync(string membershipId, string userId)
        {
            UserMembership? userMembership = await this.repository
                .All<UserMembership>()
                .FirstOrDefaultAsync(um => um.MembershipId == Guid.Parse(membershipId) && um.UserId == Guid.Parse(userId));

            Membership membership = await this.repository
                .All<Membership>()
                .FirstAsync(m => m.IsDeleted == false && m.Id == Guid.Parse(membershipId));

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
               this.repository.AllNotDeletedReadonly<UserMembership>()
                              .Where(um => um.UserId == Guid.Parse(userId))
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
                this.repository.AllNotDeletedReadonly<Membership>()
                               .Where(e => e.GymId == Guid.Parse(queryModel.GymId))
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

        public async Task<List<PayedMembershipViewModel>> GetPaymentsByGymIdForManagementAsync(string gymId, int skip = 0, int? take = null)
        {
			IQueryable<UserMembership> userMembershipsAsQuery = this.repository.AllNotDeletedReadonly<UserMembership>()
			   .Include(um => um.Membership)
			   .Include(um => um.User)
			   .Where(um => um.Membership.GymId == Guid.Parse(gymId))
			   .OrderByDescending(um => um.CreatedOn)
			   .Skip(skip);

			if (take.HasValue)
			{
				userMembershipsAsQuery = userMembershipsAsQuery.Take(take.Value);
			}

			return await userMembershipsAsQuery
						   .ProjectTo<PayedMembershipViewModel>(this.mapper.ConfigurationProvider)
						   .ToListAsync();
		}

        public async Task<List<PayedMembershipViewModel>> GetPaymentsByUserIdAsync(string userId, int skip = 0, int? take = null)
        {
			IQueryable<UserMembership> userMembershipsAsQuery = this.repository.AllNotDeletedReadonly<UserMembership>()
			   .Include(um => um.Membership)
			   .Include(um => um.User)
			   .Where(um => um.UserId == Guid.Parse(userId))
			   .OrderByDescending(um => um.CreatedOn)
			   .Skip(skip);

             if (take.HasValue)
            {
                userMembershipsAsQuery = userMembershipsAsQuery.Take(take.Value);
            }

            return await userMembershipsAsQuery
                           .ProjectTo<PayedMembershipViewModel>(this.mapper.ConfigurationProvider)
                           .ToListAsync();
		}

        public async Task<int> GetActivePaymentsCountByGymIdAsync(string gymId)
        {
            return await this.repository.AllNotDeletedReadonly<UserMembership>()
                                        .Include(um => um.Membership)
                                        .CountAsync(um => um.Membership.IsDeleted == false && um.Membership.GymId == Guid.Parse(gymId));
        }

        public async Task<int> GetAllActiveMembershipsCountByGymIdAsync(string gymId)
        {
            return await this.repository.AllNotDeletedReadonly<Membership>()
                .CountAsync(m => m.GymId == Guid.Parse(gymId));
        }
        
        public async Task<int> GetAllUserMembershipsCountByUserIdAsync(string userId)
        {
            return await this.repository.AllNotDeletedReadonly<UserMembership>()
                .CountAsync(um => um.UserId == Guid.Parse(userId));
        }

        public async Task<MembershipDetailsViewModel> GetMembershipDetailsByIdAsync(string membershipId)
        {
            Membership membership = await this.repository
                .AllNotDeletedReadonly<Membership>()
                .Where(m => m.Id == Guid.Parse(membershipId))
                .Include(m => m.Gym)
                .FirstAsync();

            MembershipDetailsViewModel membershipDetailsViewModel = this.mapper.Map<MembershipDetailsViewModel>(membership);

            return membershipDetailsViewModel;
        } 

        public async Task<UserMembership?> GetUserMembershipAsync(string userId, string membershipId)
        {
            return await this.repository.AllNotDeletedReadonly<UserMembership>()
                .FirstOrDefaultAsync(um => um.UserId == Guid.Parse(userId) && um.MembershipId == Guid.Parse(membershipId));
        }
        
        public async Task<EditMembershipInputModel> GetMembershipForEditByIdAsync(string membershipId)
		{
            Membership membershipToEdit = await this.repository
                .AllNotDeletedReadonly<Membership>()
                .FirstAsync(m => m.Id == Guid.Parse(membershipId));

            EditMembershipInputModel editMembershipInputModel = this.mapper.Map<EditMembershipInputModel>(membershipToEdit);

            return editMembershipInputModel;
		}

        public async Task<Membership?> GetMembershipByIdAsync(string membershipId)
        {
            return await this.repository.AllNotDeletedReadonly<Membership>()
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse(membershipId));
        }
        
        public async Task<bool> CheckIfMembershipExistsByIdAsync(string membershipId)
		{
            return await this.repository
                .AllNotDeletedReadonly<Membership>()
                .AnyAsync(m => m.Id == Guid.Parse(membershipId));
		}

        public async Task<int> GetAllActiveMembershipsCountAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Membership>()
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