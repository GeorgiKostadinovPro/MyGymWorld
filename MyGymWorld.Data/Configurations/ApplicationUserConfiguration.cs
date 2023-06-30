namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Common.Constants;
    using MyGymWorld.Data.Models;

    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasKey(au => au.Id);

            builder
                .HasOne(au => au.Address)
                .WithMany(a => a.Users)
                .HasForeignKey(au => au.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder
               .Property(au => au.UserName)
               .IsRequired()
               .HasMaxLength(ApplicationUserConstants.UsernameMaxLength);

            builder
                .Property(au => au.FirstName)
                .IsRequired(false)
                .HasMaxLength(ApplicationUserConstants.FirstNameMaxLength);

            builder
               .Property(au => au.LastName)
               .IsRequired(false)
               .HasMaxLength(ApplicationUserConstants.LastNameMaxLength);
        }
    }
}
