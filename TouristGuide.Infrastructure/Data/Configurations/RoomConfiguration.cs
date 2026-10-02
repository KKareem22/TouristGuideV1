using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.Property(r => r.PricePerNight)
                   .HasColumnType("decimal(18,2)");

            builder.Property(r => r.RoomNumber)
                   .HasMaxLength(20);

            builder.HasOne(r => r.Accommodation)
                   .WithMany(a => a.Rooms)
                   .HasForeignKey(r => r.AccommodationId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
