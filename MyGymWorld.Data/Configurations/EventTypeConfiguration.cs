namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Common.Constants;
    using MyGymWorld.Data.Models;

    public class EventTypeConfiguration : IEntityTypeConfiguration<EventType>
    {
        public void Configure(EntityTypeBuilder<EventType> builder)
        {
            builder.HasKey(et => et.Id);

            builder
               .Property(et => et.Name)
               .IsRequired()
               .HasMaxLength(ValidationalConstants.EventTypeConstants.NameMaxLength);
        }
    }
}
