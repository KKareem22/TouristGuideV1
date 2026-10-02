using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class TourConfiguration : IEntityTypeConfiguration<Tour>
    {
        public void Configure(EntityTypeBuilder<Tour> builder)
        {
            builder.Property(t => t.Title)
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(t => t.PricePerPerson)
                   .HasColumnType("decimal(18,2)");

            builder.Property(t => t.AverageRating)
                   .HasColumnType("decimal(3,2)");

            builder.Property(t => t.Languages)
                   .HasMaxLength(500);

            builder.Property(t => t.ExperienceType)
                   .HasMaxLength(100);

            // Guide can have many tours; restrict delete so we don't lose booking history
            builder.HasOne(t => t.GuideProfile)
                   .WithMany(g => g.Tours)
                   .HasForeignKey(t => t.GuideProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
