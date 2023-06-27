namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Common.Constants;
    using MyGymWorld.Data.Models;

    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);

            builder
                .HasOne(e => e.Gym)
                .WithMany(g => g.Events)
                .HasForeignKey(e => e.GymId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(e => e.EventType)
                .WithMany(g => g.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.EventConstants.NameMaxLength);

            builder
                .Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.EventConstants.DescriptionMaxLength);
        }
    }
}
