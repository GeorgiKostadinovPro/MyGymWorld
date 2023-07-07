namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Common;
    using MyGymWorld.Data.Models;
    using System;

    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder
                .Property(n => n.Content)
                .IsRequired()
                .HasMaxLength(ValidationalConstants.NotificatioConstants.ContentMaxLength);
        }
    }
}
