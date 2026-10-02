using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class AiRecommendationLogConfiguration : IEntityTypeConfiguration<AiRecommendationLog>
    {
        public void Configure(EntityTypeBuilder<AiRecommendationLog> builder)
        {
            builder.Property(r => r.Score)
                   .HasColumnType("decimal(5,4)");  // e.g. 0.9875

            builder.Property(r => r.Feedback)
                   .HasMaxLength(1000);
        }
    }
}
