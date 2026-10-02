using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class StayBookingConfiguration : IEntityTypeConfiguration<StayBooking>
    {
        public void Configure(EntityTypeBuilder<StayBooking> builder)
        {
            builder.Property(sb => sb.TotalPrice)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(sb => sb.TouristProfile)
                   .WithMany(t => t.StayBookings)
                   .HasForeignKey(sb => sb.TouristProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Room → Restrict (preserve booking history)
            builder.HasOne(sb => sb.Room)
                   .WithMany(r => r.StayBookings)
                   .HasForeignKey(sb => sb.RoomId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Accommodation → NoAction to avoid multiple cascade paths
            builder.HasOne(sb => sb.Accommodation)
                   .WithMany(a => a.StayBookings)
                   .HasForeignKey(sb => sb.AccommodationId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
