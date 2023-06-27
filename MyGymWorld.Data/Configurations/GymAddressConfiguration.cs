namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class GymAddressConfiguration : IEntityTypeConfiguration<GymAddress>
    {
        public void Configure(EntityTypeBuilder<GymAddress> builder)
        {
            builder.HasKey(ga => ga.Id);

            builder
                .HasOne(ga => ga.Gym)
                .WithMany(g => g.GymsAddresses)
                .HasForeignKey(ga => ga.GymId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(ga => ga.Address)
                .WithMany(g => g.GymsAddresses)
                .HasForeignKey(ga => ga.AddressId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
