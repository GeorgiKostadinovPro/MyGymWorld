namespace MyGymWorld.Data.Models
{
    using System;

    public class UserArticle
    {
        public UserArticle()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; } = null!;

        public Guid ArticleId { get; set; }

        public virtual Article Article { get; set; } = null!;
    }
}
