using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class ServiceBookingConfiguration : IEntityTypeConfiguration<ServiceBooking>
    {
        public void Configure(EntityTypeBuilder<ServiceBooking> builder)
        {
            // Prevent Cascade Delete
            builder.HasOne(sb => sb.Service)
                   .WithMany(s => s.ServiceBookings)
                   .HasForeignKey(sb => sb.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sb => sb.Tourist)
                   .WithMany(t => t.ServiceBookings)
                   .HasForeignKey(sb => sb.TouristId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
