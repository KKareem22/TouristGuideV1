using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.Property(r => r.Comment)
                   .HasMaxLength(2000);

            builder.HasOne(r => r.TouristProfile)
                   .WithMany(t => t.Reviews)
                   .HasForeignKey(r => r.TouristProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
