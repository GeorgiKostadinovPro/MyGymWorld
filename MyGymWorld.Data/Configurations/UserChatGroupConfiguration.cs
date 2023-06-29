namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    internal class UserChatGroupConfiguration : IEntityTypeConfiguration<UserChatGroup>
    {
        public void Configure(EntityTypeBuilder<UserChatGroup> builder)
        {
            builder.HasKey(ucg => ucg.Id);

            builder
                .HasOne(ucg => ucg.User)
                .WithMany(u => u.UsersChatGroups)
                .HasForeignKey(ucg => ucg.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(ucg => ucg.ChatGroup)
                .WithMany(u => u.UsersChatGroups)
                .HasForeignKey(ucg => ucg.ChatGroupId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
