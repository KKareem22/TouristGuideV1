using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class GuideBookingConfiguration : IEntityTypeConfiguration<GuideBooking>
    {
        public void Configure(EntityTypeBuilder<GuideBooking> builder)
        {
            builder.Property(gb => gb.TotalPrice)
                   .HasColumnType("decimal(18,2)");

            // Prevent Cascade Delete on GuideProfile
            builder.HasOne(gb => gb.GuideProfile)
                   .WithMany(gp => gp.Bookings)
                   .HasForeignKey(gb => gb.GuideProfileId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Prevent Cascade Delete on Tourist
            builder.HasOne(gb => gb.Tourist)
                   .WithMany(t => t.GuideBookings)
                   .HasForeignKey(gb => gb.TouristId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
