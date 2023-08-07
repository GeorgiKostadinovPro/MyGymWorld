namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Articles;
    using MyGymWorld.Web.ViewModels.Managers.Articles;

    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(CreateArticleInputModel createArticleInputModel);

        Task EditArticleAsync(string articleId, EditArticleInputModel editArticleInputModel);

        Task DeleteArticleAsync(string articleId);

        Task SubscribeUserToGymArticlesAsync(string userId, string gymId);

        Task UnsubscribeUserToGymArticlesAsync(string userId, string gymId);

        Task<IEnumerable<ArticleViewModel>> GetAllActiveArticlesFilteredAndPagedByGymIdAsync(AllArticlesForGymQueryModel queryModel);
        
        Task<int> GetAllActiveArticlesCountByGymIdAsync(string gymId);
        
        Task<ArticleDetailsViewModel> GetArticleDetailsByIdAsync(string articleId);

        Task<EditArticleInputModel> GetArticleForEditByIdAsync(string articleId);

        Task<Article?> GetArticleByIdAsync(string articleId);

        Task<bool> CheckIfArticleExistsByIdAsync(string articleId);
    }
}
