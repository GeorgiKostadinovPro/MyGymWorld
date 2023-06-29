namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Common.Constants;
    using MyGymWorld.Data.Models;

    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasKey(m => m.Id);

            builder
                .HasOne(m => m.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
               .HasOne(m => m.ChatGroup)
               .WithMany(u => u.Messages)
               .HasForeignKey(u => u.ChatGroupId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired();

            builder
                .Property(m => m.Content)
                .IsRequired(true)
                .HasMaxLength(ValidationalConstants.MessageConstants.MessageMaxLength);
        }
    }
}