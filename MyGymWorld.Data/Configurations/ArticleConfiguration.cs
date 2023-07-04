namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Common;
    using MyGymWorld.Data.Models;

    public class ArticleConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.ArticleConstants.TitleMaxLength);

            builder
                .Property(a => a.Content)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.ArticleConstants.ContentMaxLength);
        }
    }
}
