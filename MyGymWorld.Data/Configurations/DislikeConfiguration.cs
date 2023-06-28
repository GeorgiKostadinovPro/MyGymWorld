namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class DislikeConfiguration : IEntityTypeConfiguration<Dislike>
    {
        public void Configure(EntityTypeBuilder<Dislike> builder)
        {
            builder.HasKey(dl => dl.Id);

            builder
                .HasOne(dl => dl.User)
                .WithMany(u => u.Dislikes)
                .HasForeignKey(dl => dl.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(l => l.Gym)
                .WithMany(u => u.Dislikes)
                .HasForeignKey(l => l.GymId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
