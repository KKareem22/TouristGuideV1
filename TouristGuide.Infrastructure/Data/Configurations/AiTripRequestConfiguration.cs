using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class AiTripRequestConfiguration : IEntityTypeConfiguration<AiTripRequest>
    {
        public void Configure(EntityTypeBuilder<AiTripRequest> builder)
        {
            builder.Property(a => a.Destination)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(a => a.Interests)
                   .HasMaxLength(500);

            // Optional FK to Trip — no cascade, trip can exist independently
            builder.HasOne<Trip>()
                   .WithMany()
                   .HasForeignKey(a => a.GeneratedTripId)
                   .IsRequired(false)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
