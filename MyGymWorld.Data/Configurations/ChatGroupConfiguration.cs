namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class ChatGroupConfiguration : IEntityTypeConfiguration<ChatGroup>
    {
        public void Configure(EntityTypeBuilder<ChatGroup> builder)
        {
            builder.HasKey(cg => cg.Id);

            builder
                .HasOne(cg => cg.Manager)
                .WithMany(g => g.ChatGroups)
                .HasForeignKey(cg => cg.ManagerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(cg => cg.Gym)
                .WithOne(g => g.ChatGroup)
                .HasForeignKey<ChatGroup>(cg => cg.GymId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
