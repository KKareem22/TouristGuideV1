using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class GuideProfileConfiguration : IEntityTypeConfiguration<GuideProfile>
    {
        public void Configure(EntityTypeBuilder<GuideProfile> builder)
        {
            builder.Property(g => g.PricePerDay)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
