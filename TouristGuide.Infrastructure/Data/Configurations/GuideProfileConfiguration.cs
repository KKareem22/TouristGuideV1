using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class GuideProfileConfiguration : IEntityTypeConfiguration<GuideProfile>
    {
        public void Configure(EntityTypeBuilder<GuideProfile> builder)
        {
            builder.Property(g => g.PricePerPerson)
                   .HasColumnType("decimal(18,2)");

            builder.Property(g => g.AverageRating)
                   .HasColumnType("decimal(3,2)");

            builder.Property(g => g.Location)
                   .HasMaxLength(200);

            builder.Property(g => g.Languages)
                   .HasMaxLength(500);

            builder.Property(g => g.Specialties)
                   .HasMaxLength(500);
        }
    }
}
