namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Common;
    using MyGymWorld.Data.Models;

    public class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .HasOne(a => a.Town)
                .WithMany(t => t.Addresses)
                .HasForeignKey(a => a.TownId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.AddressConstants.NameMaxLength);
        }
    }
}
