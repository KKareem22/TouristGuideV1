using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class ItineraryItemConfiguration : IEntityTypeConfiguration<ItineraryItem>
    {
        public void Configure(EntityTypeBuilder<ItineraryItem> builder)
        {
            builder.Property(i => i.Title)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(i => i.EstimatedCost)
                   .HasColumnType("decimal(18,2)");

            builder.Property(i => i.IconType)
                   .HasMaxLength(50);
        }
    }
}
