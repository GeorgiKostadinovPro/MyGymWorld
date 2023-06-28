namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class Article : BaseDeletableEntityModel
    {
        public Article()
        {
            this.Id = Guid.NewGuid();

            this.ArticlesCategories = new HashSet<ArticleCategory>();
            this.UsersArticles = new HashSet<UserArticle>();
        }

        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public Guid GymId { get; set; }

        public virtual Gym Gym { get; set; } = null!;

        public virtual ICollection<ArticleCategory> ArticlesCategories { get; set; }

        public virtual ICollection<UserArticle> UsersArticles { get; set; }
    }
}
