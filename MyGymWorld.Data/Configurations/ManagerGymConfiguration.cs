namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class ManagerGymConfiguration : IEntityTypeConfiguration<ManagerGym>
    {
        public void Configure(EntityTypeBuilder<ManagerGym> builder)
        {
            builder.HasKey(mg => mg.Id);

            builder
                .HasOne(mg => mg.Manager)
                .WithMany(m => m.ManagersGyms)
                .HasForeignKey(mg => mg.ManagerId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder
                .HasOne(mg => mg.Gym)
                .WithMany(m => m.ManagersGyms)
                .HasForeignKey(mg => mg.GymId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        }
    }
}