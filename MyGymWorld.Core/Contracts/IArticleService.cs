namespace MyGymWorld.Core.Contracts
{
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Articles;

    public interface IArticleService
    {
        Task<Article> CreateArticleAsync(CreateArticleInputModel createArticleInputModel);
    }
}
