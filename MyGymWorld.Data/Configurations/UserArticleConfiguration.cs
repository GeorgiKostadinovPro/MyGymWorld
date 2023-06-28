namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    internal class UserArticleConfiguration : IEntityTypeConfiguration<UserArticle>
    {
        public void Configure(EntityTypeBuilder<UserArticle> builder)
        {
            builder.HasKey(ua => ua.Id);

            builder
                .HasOne(ua => ua.User)
                .WithMany(u => u.UsersArticles)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(ua => ua.Article)
                .WithMany(u => u.UsersArticles)
                .HasForeignKey(ua => ua.ArticleId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}