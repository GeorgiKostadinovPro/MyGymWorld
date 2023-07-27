namespace MyGymWorld.Core.Services
{
    using MyGymWorld.Core.Contracts;
    using MyGymWorld.Data.Models;
    using MyGymWorld.Web.ViewModels.Managers.Articles;
    using System;
    using System.Threading.Tasks;

    public class ArticleService : IArticleService
    {
        public Task<Article> CreateArticleAsync(CreateArticleInputModel createArticleInputModel)
        {
            throw new NotImplementedException();
        }
    }
}
