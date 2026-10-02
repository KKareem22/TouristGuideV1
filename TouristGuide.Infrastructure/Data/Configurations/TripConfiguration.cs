using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class TripConfiguration : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.Property(t => t.Name)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(t => t.EstimatedBudgetPerPerson)
                   .HasMaxLength(50);

            builder.HasOne(t => t.TouristProfile)
                   .WithMany(tp => tp.Trips)
                   .HasForeignKey(t => t.TouristProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
