namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class UserEventConfiguration : IEntityTypeConfiguration<UserEvent>
    {
        public void Configure(EntityTypeBuilder<UserEvent> builder)
        {
            builder.HasKey(ue => ue.Id);

            builder
                .HasOne(ue => ue.User)
                .WithMany(u => u.UsersEvents)
                .HasForeignKey(ue => ue.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
               .HasOne(ue => ue.Event)
               .WithMany(u => u.UsersEvents)
               .HasForeignKey(ue => ue.EventId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();
        }
    }
}
