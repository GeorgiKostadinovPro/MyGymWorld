﻿namespace MyGymWorld.Data.Configurations
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using MyGymWorld.Data.Models;

    public class GymImageConfiguration : IEntityTypeConfiguration<GymImage>
    {
        public void Configure(EntityTypeBuilder<GymImage> builder)
        {
            builder.HasKey(gi => gi.Id);

            builder
                .Property(gi => gi.Url)
                .IsRequired();
        }
    }
}