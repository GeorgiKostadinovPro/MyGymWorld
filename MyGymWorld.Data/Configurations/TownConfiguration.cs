namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Common.Constants;
    using MyGymWorld.Data.Models;

    public class TownConfiguration : IEntityTypeConfiguration<Town>
    {
        public void Configure(EntityTypeBuilder<Town> builder)
        {
            builder.HasKey(t => t.Id);

            builder
                .HasOne(t => t.Country)
                .WithMany(c => c.Towns)
                .HasForeignKey(t => t.CountryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.TownConstants.NameMaxLength);

            builder
               .Property(t => t.Population)
               .IsRequired();

            builder
                .Property(t => t.ZipCode)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.TownConstants.ZipCodeMaxLength);
        }
    }
}
