namespace MyGymWorld.Data.Models
{
    using MyGymWorld.Data.Common.Models;
    using System;

    public class ArticleCategory : BaseDeletableEntityModel
    {
        public ArticleCategory()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; } = null!;

        public Guid CategoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
    }
}
