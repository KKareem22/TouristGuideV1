using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class AccommodationConfiguration : IEntityTypeConfiguration<Accommodation>
    {
        public void Configure(EntityTypeBuilder<Accommodation> builder)
        {
            builder.Property(a => a.Name)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(a => a.City)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(a => a.Country)
                   .HasMaxLength(100)
                   .IsRequired();

            builder.Property(a => a.PricePerNight)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.AverageRating)
                   .HasColumnType("decimal(3,2)");

            builder.Property(a => a.Currency)
                   .HasMaxLength(10)
                   .HasDefaultValue("USD");
        }
    }
}
