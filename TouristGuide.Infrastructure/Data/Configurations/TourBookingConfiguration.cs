using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class TourBookingConfiguration : IEntityTypeConfiguration<TourBooking>
    {
        public void Configure(EntityTypeBuilder<TourBooking> builder)
        {
            builder.Property(tb => tb.TotalPrice)
                   .HasColumnType("decimal(18,2)");

            builder.HasOne(tb => tb.TouristProfile)
                   .WithMany(t => t.TourBookings)
                   .HasForeignKey(tb => tb.TouristProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tb => tb.Tour)
                   .WithMany(t => t.TourBookings)
                   .HasForeignKey(tb => tb.TourId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
