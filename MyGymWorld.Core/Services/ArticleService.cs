namespace MyGymWorld.Core.Services
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

        public async Task<Article> DeleteArticleAsync(string articleId)
        {
            Article? articleToDelete = await this.repository.All<Article>(a => a.IsDeleted == false && a.Id == Guid.Parse(articleId))
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

            return articleToDelete;
        }

        public async Task<IEnumerable<ArticleViewModel>> GetAllActiveArticlesFilteredAndPagedByGymIdAsync(AllArticlesForGymQueryModel queryModel)
        {
            IQueryable<Article> articlesAsQuery =
                this.repository.AllReadonly<Article>(a => a.IsDeleted == false && a.GymId == Guid.Parse(queryModel.GymId))
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

            switch (queryModel.ArticlesSorting)
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
            return await this.repository.AllReadonly<Article>(a => a.IsDeleted == false && a.GymId == Guid.Parse(gymId))
                .CountAsync();
        }

		public async Task<ArticleDetailsViewModel> GetArticleDetailsByIdAsync(string articleId)
		{
            Article articleToDisplay = await this.repository.AllReadonly<Article>(a => a.IsDeleted == false && a.Id == Guid.Parse(articleId))
                .Include(a => a.Gym)
                .FirstAsync();

            ArticleDetailsViewModel articleDetailsViewModel = this.mapper.Map<ArticleDetailsViewModel>(articleToDisplay);

            return articleDetailsViewModel;
		}

		public async Task<bool> CheckIfArticleExistsByIdAsync(string articleId)
		{
            return await this.repository.AllReadonly<Article>(a => a.IsDeleted == false && a.Id == Guid.Parse(articleId))
                .AnyAsync();
		}

        public async Task<Article?> GetArticleByIdAsync(string articleId)
        {
            return await this.repository.AllReadonly<Article>(a => a.IsDeleted == false && a.Id == Guid.Parse(articleId))
                .FirstOrDefaultAsync();
        }
    }
}
