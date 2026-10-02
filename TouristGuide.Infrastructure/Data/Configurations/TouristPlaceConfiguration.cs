using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class TouristPlaceConfiguration : IEntityTypeConfiguration<TouristPlace>
    {
        public void Configure(EntityTypeBuilder<TouristPlace> builder)
        {
            builder.Property(p => p.Name)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(p => p.Country)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(p => p.EntryFee)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.AverageRating)
                   .HasColumnType("decimal(3,2)");

            builder.Property(p => p.Tags)
                   .HasMaxLength(500);
        }
    }
}
