namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;

    public class Category : BaseDeletableEntityModel
    {
        public Category()
        {
            this.Id = Guid.NewGuid();

            this.ArticlesCategories = new HashSet<ArticleCategory>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public virtual ICollection<ArticleCategory> ArticlesCategories { get; set;}
    }
}
