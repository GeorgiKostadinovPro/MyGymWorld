namespace MyGymWorld.Web.ViewModels.Articles
{
    using MyGymWorld.Web.ViewModels.Events.Enums;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using MyGymWorld.Data.Models;

    using static MyGymWorld.Common.GlobalConstants;
    using MyGymWorld.Web.ViewModels.Articles.Enums;

    public class AllArticlesForGymQueryModel
    {
        public AllArticlesForGymQueryModel()
        {
            this.ArticlesPerPage = ArticleConstants.ArticlesPerPage;
            this.CurrentPage = ArticleConstants.DefaultPage;
            this.Categories = new HashSet<Category>();

            this.Articles = new HashSet<ArticleViewModel>();
        }

        public string GymId { get; set; } = null!;

        public int ArticlesPerPage { get; set; }

        public string? CategoryId { get; set; } = null!;

        [Display(Name = "Search by text")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Sort Events By")]
        public ArticlesSorting ArticlesSorting { get; set; }

        public int CurrentPage { get; set; }

        public int TotalArticlesCount { get; set; }

        public IEnumerable<Category> Categories { get; set; }

        public IEnumerable<ArticleViewModel> Articles { get; set; }
    }
}