namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Common;
    using MyGymWorld.Data.Models;

    public class GymConfiguration : IEntityTypeConfiguration<Gym>
    {
        public void Configure(EntityTypeBuilder<Gym> builder)
        {
            builder.HasKey(g => g.Id);

            builder
                .HasOne(g => g.Manager)
                .WithMany(m => m.Gyms)
                .HasForeignKey(g => g.ManagerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();


            builder
                .HasOne(g => g.Address)
                .WithMany(m => m.Gyms)
                .HasForeignKey(g => g.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.GymConstants.NameMaxLength);

            builder
                .Property(g => g.PhoneNumber)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.GymConstants.PhoneNumberMaxLength);

            builder
                .Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.GymConstants.DescriptionMaxLength);
        }
    }
}
