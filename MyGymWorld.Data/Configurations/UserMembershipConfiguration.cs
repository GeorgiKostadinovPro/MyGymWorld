namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using MyGymWorld.Data.Models;

    public class UserMembershipConfiguration : IEntityTypeConfiguration<UserMembership>
    {
        public void Configure(EntityTypeBuilder<UserMembership> builder)
        {
            builder.HasKey(um => um.Id);

            builder
                .HasOne(um => um.User)
                .WithMany(u => u.UsersMemberships)
                .HasForeignKey(um => um.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(um => um.Membership)
                .WithMany(u => u.UsersMemberships)
                .HasForeignKey(um => um.MembershipId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}
