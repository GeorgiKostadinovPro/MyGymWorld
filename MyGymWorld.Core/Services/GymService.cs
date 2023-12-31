﻿namespace MyGymWorld.Core.Services
{
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Models.Enums;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Managers.Gyms;
    using MyGymWorld.Web.ViewModels.Gyms;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using MyGymWorld.Web.ViewModels.Gyms.Enums;
    using MyGymWorld.Common;

    public class GymService : IGymService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly IEventService eventService;
        private readonly IArticleService articleService;
        private readonly IMembershipService membershipService;
        private readonly ILikeService likeService;
        private readonly IDislikeService dislikeService;
        private readonly ICommentService commentService;

        private readonly IAddressService addressService;

        public GymService(
            IMapper _mapper,
            IRepository _repository,
            IEventService _eventService,
            IArticleService _articleService,
            IMembershipService _membershipService,
            ILikeService _likeService,
            IDislikeService _dislikeService,
            ICommentService _commentService,
            IAddressService _addressService)
        {
            this.mapper = _mapper;
            this.repository = _repository;

            this.eventService = _eventService;
            this.articleService = _articleService;
            this.membershipService = _membershipService;
            this.likeService = _likeService;
            this.dislikeService = _dislikeService;
            this.commentService = _commentService;

            this.addressService = _addressService;
        }

        public async Task CreateGymAsync(string managerId, CreateGymInputModel createGymInputModel)
        {
            Gym gym = new Gym
            {
                ManagerId = Guid.Parse(managerId),
                Name = createGymInputModel.Name,
                Email = createGymInputModel.Email,
                PhoneNumber = createGymInputModel.PhoneNumber,
                Description = createGymInputModel.Description,
                LogoUri = createGymInputModel.LogoResultParams!.SecureUri!.AbsoluteUri,
                LogoPublicId = createGymInputModel.LogoResultParams.PublicId,
                WebsiteUrl = createGymInputModel.WebsiteUrl,
                GymType = Enum.Parse<GymType>(createGymInputModel.GymType),
                CreatedOn = DateTime.UtcNow
            };
            
            Address? address = await this.addressService.GetAddressByNameAsync(createGymInputModel.Address);

            if (address != null)
            {
                gym.AddressId = address.Id;
            }
            else
            {
                Address createdAddress = await this.addressService.CreateAddressAsync(createGymInputModel.Address, createGymInputModel.TownId);

                gym.AddressId = createdAddress.Id;
            }  
            
            foreach (var galleryImageResultParams in createGymInputModel.GalleryImagesResultParams)
            {
                string uri = galleryImageResultParams.SecureUri.AbsoluteUri;
                string publicId = galleryImageResultParams.PublicId;

                gym.GymImages.Add(new GymImage
                {
                    GymId = gym.Id,
                    Uri = uri,
                    PublicId = publicId,
                    CreatedOn = DateTime.UtcNow,
                });
            }

            await this.repository.AddAsync(gym);
            await this.repository.SaveChangesAsync();
        }

        public async Task EditGymAsync(string gymId, EditGymInputModel editGymInputModel)
        {
            Gym? gymToEdit = await this.repository.AllNotDeleted<Gym>()
                .Where(g => g.Id.ToString() == gymId)
                .Include(g => g.Address)
                .ThenInclude(a => a.Town)
                .FirstOrDefaultAsync();

            if (gymToEdit != null)
            {
				gymToEdit.Name = editGymInputModel.Name;
				gymToEdit.Email = editGymInputModel.Email;
				gymToEdit.PhoneNumber = gymToEdit.PhoneNumber;
				gymToEdit.WebsiteUrl = editGymInputModel.WebsiteUrl;
				gymToEdit.Description = editGymInputModel.Description;
				gymToEdit.GymType = Enum.Parse<GymType>(editGymInputModel.GymType);

				Address? address = await this.addressService.GetAddressByNameAsync(editGymInputModel.Address);

				if (address != null)
				{
					gymToEdit.AddressId = address.Id;
				}
				else
				{
					Address createdAddress = await this.addressService.CreateAddressAsync(editGymInputModel.Address, editGymInputModel.TownId);

					gymToEdit.AddressId = createdAddress.Id;
				}

				if (editGymInputModel.LogoResultParams != null)
				{
					gymToEdit.LogoUri = editGymInputModel.LogoResultParams!.SecureUri!.AbsoluteUri;
					gymToEdit.LogoPublicId = editGymInputModel.LogoResultParams.PublicId;
				}

				if (editGymInputModel.GalleryImagesResultParams.Count > 0)
				{
					foreach (var galleryImageResultParams in editGymInputModel.GalleryImagesResultParams)
					{
						string uri = galleryImageResultParams.SecureUri.AbsoluteUri;
						string publicId = galleryImageResultParams.PublicId;

						gymToEdit.GymImages.Add(new GymImage
						{
							GymId = gymToEdit.Id,
							Uri = uri,
							PublicId = publicId,
							CreatedOn = DateTime.UtcNow,
						});
					}
				}

				await this.repository.SaveChangesAsync();
			}
        }

        public async Task DeleteGymAsync(string gymId)
        {
            Gym? gymToDelete = await this.repository.AllNotDeleted<Gym>()
                .Where(g => g.Id.ToString() == gymId)
                .Include(g => g.Events)
                .Include(g => g.Articles)
                .Include(g => g.Memberships)
                .Include(g => g.Likes)
                .Include(g => g.Dislikes)
                .Include(g => g.Comments)
                .Include(g => g.UsersGyms)
                .Include(g => g.GymImages)
                .FirstOrDefaultAsync();

            if (gymToDelete != null)
            {
				gymToDelete.IsDeleted = true;
				gymToDelete.DeletedOn = DateTime.UtcNow;

				foreach (Event @event in gymToDelete.Events)
				{
					await this.eventService.DeleteEventAsync(@event.Id.ToString());
				}

				foreach (Article article in gymToDelete.Articles)
				{
					await this.articleService.DeleteArticleAsync(article.Id.ToString());
				}

				foreach (Membership membership in gymToDelete.Memberships)
				{
					await this.membershipService.DeleteMembershipAsync(membership.Id.ToString());
				}

				foreach (Like like in gymToDelete.Likes)
				{
					await this.likeService.DeleteLikeAsync(like.Id.ToString());
				}

				foreach (Dislike dislike in gymToDelete.Dislikes)
				{
					await this.dislikeService.DeleteDislikeAsync(dislike.Id.ToString());
				}

				foreach (Comment comment in gymToDelete.Comments)
				{
					await this.commentService.DeleteCommentAsync(comment.Id.ToString());
				}

				IEnumerable<UserGym> userGymsToDelete = gymToDelete.UsersGyms
					.Where(ug => ug.IsDeleted == false)
					.ToArray();

				foreach (var userGym in userGymsToDelete)
				{
					userGym.IsDeleted = true;
					userGym.DeletedOn = DateTime.UtcNow;
				}

				IEnumerable<GymImage> gymImagesToDelete = gymToDelete.GymImages
					.Where(gi => gi.IsDeleted == false)
					.ToArray();

				foreach (var gymImage in gymImagesToDelete)
				{
					gymImage.IsDeleted = true;
					gymImage.DeletedOn = DateTime.UtcNow;
				}

				await this.repository.SaveChangesAsync();
			}
        }

        public async Task AddGymToUserAsync(string gymId, string userId)
        {
            UserGym? userGym = await this.repository.All<UserGym>()
                .Where(ug => ug.GymId.ToString() == gymId && ug.UserId.ToString() == userId)
                .Include(ug => ug.User)
                   .ThenInclude(u => u.UsersEvents)
                .Include(ug => ug.User)
                   .ThenInclude(u => u.UsersMemberships)
                .FirstOrDefaultAsync();

            if (userGym == null)
            {
                userGym = new UserGym
                {
                    GymId = Guid.Parse(gymId),
                    UserId = Guid.Parse(userId),
                    CreatedOn = DateTime.UtcNow
                };

                await this.repository.AddAsync(userGym);
            }
            else
            {
                if (userGym.IsDeleted == false)
                {
                    throw new InvalidOperationException(ExceptionConstants.GymErrors.GymAlreadyJoined);
                }
               
                 userGym.IsDeleted = false;
                 userGym.DeletedOn = null;
                 
                 foreach (UserEvent userEvent in userGym.User.UsersEvents
                     .Where(ue => ue.IsDeleted == true))
                 {
                     userEvent.IsDeleted = false;
                     userEvent.DeletedOn = null;
                 }
                 
                 foreach (UserMembership userMembership in userGym.User.UsersMemberships
                      .Where(ue => ue.IsDeleted == true))
                 {
                     userMembership.IsDeleted = false;
                     userMembership.DeletedOn = null;
                 }
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task RemoveGymFromUserAsync(string gymId, string userId)
        {
            UserGym? userGym = await this.repository.AllNotDeleted<UserGym>()
                .Where(ug => ug.GymId.ToString() == gymId && ug.UserId.ToString() == userId)
                .Include(ug => ug.User)
                    .ThenInclude(u => u.UsersEvents)
                .Include(ug => ug.User)
                    .ThenInclude(u => u.UsersMemberships)
                .FirstOrDefaultAsync();

            if (userGym == null)
            {
                throw new InvalidOperationException(ExceptionConstants.GymErrors.GymNotJoinedToBeLeft);
            }
            else
            {
                userGym.IsDeleted = true;
                userGym.DeletedOn = DateTime.UtcNow;
                
                foreach (UserEvent userEvent in userGym.User.UsersEvents
                    .Where(ue => ue.IsDeleted == false))
                {
                    userEvent.IsDeleted = true;
                    userEvent.DeletedOn = DateTime.UtcNow;
                }

                foreach (UserMembership userMembership in userGym.User.UsersMemberships
                    .Where(um => um.IsDeleted == false))
                {
                    userMembership.IsDeleted = true;
                    userMembership.DeletedOn = DateTime.UtcNow;
                }

                await this.repository.SaveChangesAsync();
            }
        }

        public async Task<List<GymViewModel>> GetActiveOrDeletedForManagementAsync(string managerId, bool isDeleted, int skip = 0, int? take = null)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllReadonly<Gym>()
                .Where(g => g.IsDeleted == isDeleted && g.ManagerId.ToString() == managerId)
                .OrderByDescending(g => g.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                gymsAsQuery = gymsAsQuery.Take(take.Value);
            }

            return await gymsAsQuery
                .ProjectTo<GymViewModel>(this.mapper.ConfigurationProvider)
                .ToListAsync();
        }
        
        public async Task<List<GymViewModel>> GetActiveOrDeletedForAdministrationAsync(bool isDeleted, int skip = 0, int? take = null)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllReadonly<Gym>()
                .Where(g => g.IsDeleted == isDeleted)
                .OrderByDescending(g => g.CreatedOn)
                .Skip(skip);

            if (take.HasValue)
            {
                gymsAsQuery = gymsAsQuery.Take(take.Value);
            }

            return await gymsAsQuery
               .ProjectTo<GymViewModel>(this.mapper.ConfigurationProvider)
               .ToListAsync();
        }

        public async Task<int> GetActiveOrDeletedGymsCountForManagementAsync(string managerId, bool isDeleted)
        {
            return await this.repository
                .AllReadonly<Gym>()
                .CountAsync(g => g.IsDeleted == isDeleted && g.ManagerId.ToString() == managerId);
        }

        public async Task<int> GetActiveOrDeletedGymsCountForAdministrationAsync(bool isDeleted)
        {
            return await this.repository
                .AllReadonly<Gym>()
                .CountAsync(g => g.IsDeleted == isDeleted);
        }

        public async Task<int> GetActiveGymsCountAsync()
        {
            return await this.repository
                .AllNotDeletedReadonly<Gym>()
                .CountAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetTop10NewestActiveGymsAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Gym>()
                .OrderByDescending(g => g.CreatedOn)
                .Take(10)
                .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetTop10MostLikedActiveGymsAsync()
        {
            return await this.repository.AllNotDeletedReadonly<Gym>()
                .OrderByDescending(g => g.Likes.Count)
                .Take(10)
                .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                .ToArrayAsync();
        }

        public async Task<IEnumerable<DisplayGymViewModel>> GetAllActiveGymsFilteredAndPagedAsync(AllGymsQueryModel queryModel)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllNotDeletedReadonly<Gym>()
                .Include(g => g.Address);

            if (!string.IsNullOrWhiteSpace(queryModel.GymType))
            {
                gymsAsQuery = gymsAsQuery
                    .Where(g => g.GymType == Enum.Parse<GymType>(queryModel.GymType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                gymsAsQuery = gymsAsQuery
                    .Where(g => EF.Functions.Like(g.Name, wildCard)
                    || EF.Functions.Like(g.Description, wildCard)
                    || EF.Functions.Like(g.Address.Name, wildCard));
            }

            switch (queryModel.Sorting)
            {
                case GymsSorting.Newest:
                    gymsAsQuery = gymsAsQuery
                        .OrderByDescending(g => g.CreatedOn);
                    break;
                case GymsSorting.Oldest:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.CreatedOn);
                    break;
                case GymsSorting.LikesAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Likes.Count);
                    break;
                case GymsSorting.LikesDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Likes.Count);
                    break;
                case GymsSorting.CommentsAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Comments.Count);
                    break;
                case GymsSorting.CommentsDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Comments.Count);
                    break;
                default:
                    break;
            }

            IEnumerable<DisplayGymViewModel> gymsToDisplay
                = await gymsAsQuery
                             .Skip((queryModel.CurrentPage - 1) * queryModel.GymsPerPage)
                             .Take(queryModel.GymsPerPage)
                             .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                             .ToArrayAsync();

            return gymsToDisplay;
        }

        public async Task<GymDetailsViewModel> GetGymDetailsByIdAsync(string gymId)
        {
            Gym gym = await this.repository.AllNotDeletedReadonly<Gym>()
                .Where(g => g.Id.ToString() == gymId)
                .Include(g => g.Manager)
                   .ThenInclude(m => m.User)
                .Include(g => g.Address)
                   .ThenInclude(a => a.Town)
                      .ThenInclude(t => t.Country)
                .Include(g => g.GymImages)
                .Include(g => g.UsersGyms)
                .Include(g => g.Likes)
                .Include(g => g.Dislikes)
                .Include(g => g.Comments)
                .Include(g => g.Events)
                .Include(g => g.Articles)
                .Include(g => g.Memberships)
                .FirstAsync();

            GymDetailsViewModel gymDetailsViewModel = this.mapper.Map<GymDetailsViewModel>(gym);

            return gymDetailsViewModel;
        }

        public async Task<EditGymInputModel> GetGymForEditByIdAsync(string gymId)
		{
            Gym gymForEdit = await this.repository.AllNotDeletedReadonly<Gym>()
                .Where(g => g.IsDeleted == false && g.Id.ToString() == gymId)
                .Include(g => g.Address)
                .ThenInclude(a => a.Town)
				.FirstAsync();

            EditGymInputModel editGymInputModel = this.mapper.Map<EditGymInputModel>(gymForEdit);

            return editGymInputModel;
		} 
       
        public async Task<IEnumerable<DisplayGymViewModel>> GetAllUserJoinedGymsFilteredAndPagedAsync(string userId, AllUserJoinedGymsQueryModel queryModel)
        {
            IQueryable<Gym> gymsAsQuery = this.repository
                .AllNotDeletedReadonly<UserGym>()
                .Where(ug => ug.UserId.ToString() == userId)
                .Include(ug => ug.Gym)
                .ThenInclude(g => g.Address)
                .Select(ug => ug.Gym);

            if (!string.IsNullOrWhiteSpace(queryModel.GymType))
            {
                gymsAsQuery = gymsAsQuery
                    .Where(g => g.GymType == Enum.Parse<GymType>(queryModel.GymType));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                gymsAsQuery = gymsAsQuery
                    .Where(g => EF.Functions.Like(g.Name, wildCard)
                    || EF.Functions.Like(g.Description, wildCard)
                    || EF.Functions.Like(g.Address.Name, wildCard));
            }

            switch (queryModel.Sorting)
            {
                case GymsSorting.Newest:
                    gymsAsQuery = gymsAsQuery
                        .OrderByDescending(g => g.CreatedOn);
                    break;
                case GymsSorting.Oldest:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.CreatedOn);
                    break;
                case GymsSorting.LikesAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Likes.Count);
                    break;
                case GymsSorting.LikesDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Likes.Count);
                    break;
                case GymsSorting.CommentsAscending:
                    gymsAsQuery = gymsAsQuery
                       .OrderBy(g => g.Comments.Count);
                    break;
                case GymsSorting.CommentsDescending:
                    gymsAsQuery = gymsAsQuery
                       .OrderByDescending(g => g.Comments.Count);
                    break;
                default:
                    break;
            }

            IEnumerable<DisplayGymViewModel> gymsToDisplay
                = await gymsAsQuery
                             .Skip((queryModel.CurrentPage - 1) * queryModel.GymsPerPage)
                             .Take(queryModel.GymsPerPage)
                             .ProjectTo<DisplayGymViewModel>(this.mapper.ConfigurationProvider)
                             .ToArrayAsync();

            return gymsToDisplay;
        }

        public async Task<int> GetAllUserJoinedGymsCountAsync(string userId)
        {
            return await this.repository.AllNotDeletedReadonly<UserGym>()
                .CountAsync(ug => ug.UserId.ToString() == userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersWhoAreSubscribedForGymArticlesAsync(string gymId)
        {
            return await this.repository.AllNotDeletedReadonly<UserGym>()
                .Where(ug => ug.GymId.ToString() == gymId && ug.IsSubscribedForArticles == true)
                .Include(ug => ug.User)
                .Select(ug => ug.User)
                .ToArrayAsync();
        }

        public async Task<bool> CheckIfUserIsSubscribedForGymArticles(string userId, string gymId)
        {
            bool isSubscribedForArticles = await this.repository.AllNotDeletedReadonly<UserGym>()
                .AnyAsync(ug => ug.UserId.ToString() == userId && ug.GymId.ToString() == gymId && ug.IsSubscribedForArticles == true);

            return isSubscribedForArticles;
        }

        public async Task<bool> CheckIfGymExistsByIdAsync(string gymId)
		{
            return await this.repository.AllNotDeletedReadonly<Gym>()
                .AnyAsync(g => g.Id.ToString() == gymId);
		}

        public async Task<bool> CheckIfGymIsManagedByManagerAsync(string gymId, string mananerId)
        {
            return await this.repository.AllNotDeletedReadonly<Gym>()
                .AnyAsync(g => g.Id.ToString() == gymId && g.ManagerId.ToString() == mananerId);
        }

        public async Task<bool> CheckIfGymIsJoinedByUserAsync(string gymId, string userId)
        {
            return await this.repository.AllNotDeletedReadonly<UserGym>()
                .AnyAsync(ug => ug.GymId.ToString() == gymId && ug.UserId.ToString() == userId);
        }

        public async Task<Gym?> GetGymByIdAsync(string gymId)
        {
            return await this.repository.AllNotDeletedReadonly<Gym>()
                .FirstOrDefaultAsync(g => g.Id.ToString() == gymId);
        }

        public IEnumerable<string> GetAllGymTypes()
        {
            IEnumerable<string> gymTypes = Enum.GetValues(typeof(GymType))
                .Cast<GymType>()
                .Select(gt => gt.ToString())
                .ToImmutableArray();

            return gymTypes;    
        }
    }
}