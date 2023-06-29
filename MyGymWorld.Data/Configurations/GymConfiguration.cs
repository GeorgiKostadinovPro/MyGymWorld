namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Common.Constants;
    using MyGymWorld.Data.Models;

    public class GymConfiguration : IEntityTypeConfiguration<Gym>
    {
        public void Configure(EntityTypeBuilder<Gym> builder)
        {
            builder.HasKey(g => g.Id);

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
