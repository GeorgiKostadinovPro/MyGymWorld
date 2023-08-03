﻿namespace MyGymWorld.Core.Services
{
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Data.Repositories;
    using MyGymWorld.Web.ViewModels.Articles;
    using MyGymWorld.Web.ViewModels.Managers.Articles;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MyGymWorld.Web.ViewModels.Articles.Enums;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;

    public class ArticleService : IArticleService
    {
        private readonly IMapper mapper;
        private readonly IRepository repository;

        private readonly ICategoryService categoryService;

        public ArticleService(
            IMapper mapper,
            IRepository _repository, 
            ICategoryService _categoryService)
        {
            this.mapper = mapper;
            this.repository = _repository;

            this.categoryService = _categoryService;

        }

        public async Task<Article> CreateArticleAsync(CreateArticleInputModel createArticleInputModel)
        {
            Article article = this.mapper.Map<Article>(createArticleInputModel);
            
            article.CreatedOn = DateTime.UtcNow;

            foreach (string categoryId in createArticleInputModel.CategoryIds)
            {
                article.ArticlesCategories.Add(new ArticleCategory
                {
                    ArticleId = article.Id,
                    CategoryId = Guid.Parse(categoryId)
                });
            }

            await this.repository.AddAsync(article);
            await this.repository.SaveChangesAsync();

            return article;
        }

        public async Task EditArticleAsync(string articleId, EditArticleInputModel editArticleInputModel)
        {
            Article? articleToEdit = await this.repository
                .AllNotDeleted<Article>()
                .Where(a => a.Id == Guid.Parse(articleId))
                .Include(a => a.ArticlesCategories)
                .FirstOrDefaultAsync();

            if (articleToEdit != null)
            {
                articleToEdit.Title = editArticleInputModel.Title;
                articleToEdit.Content = editArticleInputModel.Content;
                articleToEdit.ModifiedOn = DateTime.UtcNow;

                foreach (ArticleCategory articleCategory in articleToEdit.ArticlesCategories.Where(ac => ac.IsDeleted == false))
                {
                    articleCategory.IsDeleted = true;
                    articleCategory.DeletedOn = DateTime.UtcNow;
                }

                foreach (string categoryId in editArticleInputModel.CategoryIds)
                {
                    ArticleCategory? articleCategory = await this.repository.All<ArticleCategory>()
                        .FirstOrDefaultAsync(ac => ac.ArticleId == articleToEdit.Id && ac.CategoryId == Guid.Parse(categoryId));

                    if (articleCategory != null)
                    {
                        articleCategory.IsDeleted = false;
                        articleCategory.DeletedOn = null;
                    }
                    else
                    {
                        articleCategory = new ArticleCategory
                        {
                            ArticleId = articleToEdit.Id,
                            CategoryId = Guid.Parse(categoryId),
                            CreatedOn = DateTime.UtcNow
                        };

                        await this.repository.AddAsync(articleCategory);
                    }
                }

                await this.repository.SaveChangesAsync();
            }
        }

        public async Task DeleteArticleAsync(string articleId)
        {
            Article? articleToDelete = await this.repository.AllNotDeleted<Article>()
                .Where(a => a.Id == Guid.Parse(articleId))
                .Include(a => a.ArticlesCategories)
                .FirstOrDefaultAsync();

            if (articleToDelete != null)
            {
                articleToDelete.IsDeleted = true;
                articleToDelete.DeletedOn = DateTime.UtcNow;

                foreach (var articleCategory in articleToDelete.ArticlesCategories)
                {
                    articleCategory.IsDeleted = true;
                    articleCategory.DeletedOn = DateTime.UtcNow;
                }

                await this.repository.SaveChangesAsync();
            }
        }

        public async Task SubscribeUserToGymArticlesAsync(string userId, string gymId)
        {
            UserGym? userGym = await this.repository.AllNotDeleted<UserGym>()
                .FirstOrDefaultAsync(ug => ug.UserId == Guid.Parse(userId) && ug.GymId == Guid.Parse(gymId));

            if (userGym != null && userGym.IsSubscribedForArticles == false)
            {
                userGym.IsSubscribedForArticles = true;
                userGym.ModifiedOn = DateTime.UtcNow;
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task UnsubscribeUserToGymArticlesAsync(string userId, string gymId)
        {
            UserGym userGym = await this.repository.AllNotDeleted<UserGym>()
               .FirstAsync(ug => ug.UserId == Guid.Parse(userId) && ug.GymId == Guid.Parse(gymId));

            if (userGym.IsSubscribedForArticles == true)
            {
                userGym.IsSubscribedForArticles = false;
                userGym.ModifiedOn = DateTime.UtcNow;
            }

            await this.repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ArticleViewModel>> GetAllActiveArticlesFilteredAndPagedByGymIdAsync(AllArticlesForGymQueryModel queryModel)
        {
            IQueryable<Article> articlesAsQuery =
                this.repository.AllNotDeletedReadonly<Article>()
                               .Where(a => a.GymId == Guid.Parse(queryModel.GymId))
                               .Include(a => a.ArticlesCategories)
                                  .ThenInclude(ac => ac.Category)
                               .Include(a => a.Gym)
                               .ThenInclude(g => g.Manager)
                               .ThenInclude(m => m.User);

            if (!string.IsNullOrWhiteSpace(queryModel.CategoryId))
            {
                articlesAsQuery = articlesAsQuery
                    .Where(a => a.ArticlesCategories.Any(ac => ac.CategoryId == Guid.Parse(queryModel.CategoryId)));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SearchTerm))
            {
                string wildCard = $"%{queryModel.SearchTerm.ToLower()}%";

                articlesAsQuery = articlesAsQuery
                    .Where(e => EF.Functions.Like(e.Title, wildCard)
                    || EF.Functions.Like(e.Content, wildCard)
                    || EF.Functions.Like(e.Gym.Name, wildCard));
            }

            switch (queryModel.Sorting)
            {
                case ArticlesSorting.Newest:
                    articlesAsQuery = articlesAsQuery
                        .OrderByDescending(e => e.CreatedOn);
                    break;
                case ArticlesSorting.Oldest:
                    articlesAsQuery = articlesAsQuery
                       .OrderBy(e => e.CreatedOn);
                    break;
            }

            IEnumerable<ArticleViewModel> articlesToDisplay
                = await articlesAsQuery
                               .Skip((queryModel.CurrentPage - 1) * queryModel.ArticlesPerPage)
                               .Take(queryModel.ArticlesPerPage)
                               .ProjectTo<ArticleViewModel>(this.mapper.ConfigurationProvider)
                               .ToArrayAsync();

            return articlesToDisplay;
        }
        
        public async Task<int> GetAllActiveArticlesCountByGymIdAsync(string gymId)
        {
            return await this.repository.AllNotDeletedReadonly<Article>()
                .CountAsync(a => a.GymId == Guid.Parse(gymId));
        }

		public async Task<ArticleDetailsViewModel> GetArticleDetailsByIdAsync(string articleId)
		{
            Article articleToDisplay = await this.repository.AllNotDeletedReadonly<Article>()
                .Where(a => a.Id == Guid.Parse(articleId))
                .Include(a => a.Gym)
                .FirstAsync();

            ArticleDetailsViewModel articleDetailsViewModel = this.mapper.Map<ArticleDetailsViewModel>(articleToDisplay);

            return articleDetailsViewModel;
		}

		public async Task<bool> CheckIfArticleExistsByIdAsync(string articleId)
		{
            return await this.repository.AllNotDeletedReadonly<Article>()
                .AnyAsync(a => a.Id == Guid.Parse(articleId));
		}

        public async Task<Article?> GetArticleByIdAsync(string articleId)
        {
            return await this.repository.AllNotDeletedReadonly<Article>()
                .FirstOrDefaultAsync(a => a.Id == Guid.Parse(articleId));
        }

        public async Task<EditArticleInputModel> GetArticleForEditByIdAsync(string articleId)
        {
            Article articleToEdit = await this.repository.AllNotDeletedReadonly<Article>()
                .FirstAsync(a => a.Id == Guid.Parse(articleId));

            EditArticleInputModel editArticleInputModel = this.mapper.Map<EditArticleInputModel>(articleToEdit);

            return editArticleInputModel;
        }

        public async Task<bool> CheckIfUserIsSubscribedForGymArticles(string userId, string gymId)
        {
            bool isSubscribedForArticles = await this.repository.AllNotDeletedReadonly<UserGym>()
                .AnyAsync(ug => ug.UserId == Guid.Parse(userId) && ug.GymId == Guid.Parse(gymId) && ug.IsSubscribedForArticles == true);

            return isSubscribedForArticles;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersWhoAreSubscribedForGymArticlesAsync(string gymId)
        {
            return await this.repository.AllNotDeletedReadonly<UserGym>()
                .Where(ug => ug.GymId == Guid.Parse(gymId) && ug.IsSubscribedForArticles == true)
                .Include(ug => ug.User)
                .Select(ug => ug.User)
                .ToArrayAsync();
        }
    }
}
