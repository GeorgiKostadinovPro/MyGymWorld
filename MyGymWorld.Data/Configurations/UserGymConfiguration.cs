namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class UserGymConfiguration : IEntityTypeConfiguration<UserGym>
    {
        public void Configure(EntityTypeBuilder<UserGym> builder)
        {
            builder.HasKey(ug => ug.Id);

            builder
                .HasOne(ug => ug.User)
                .WithMany(u => u.UsersGyms)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
               .HasOne(ug => ug.Gym)
               .WithMany(u => u.UsersGyms)
               .HasForeignKey(ug => ug.GymId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

            builder
                .Property(ug => ug.IsSubscribedForArticles)
                .HasDefaultValue(false);
        }
    }
}
