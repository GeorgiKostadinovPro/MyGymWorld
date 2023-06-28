namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
             .HasOne(c => c.Gym)
             .WithMany(u => u.Comments)
             .HasForeignKey(c => c.GymId)
             .OnDelete(DeleteBehavior.Restrict)
             .IsRequired();

            builder
                .Property(c => c.ParentId)
                .IsRequired(false);
        }
    }
}
