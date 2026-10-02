using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
    {
        public void Configure(EntityTypeBuilder<UserPreference> builder)
        {
            // One preference record per user
            builder.HasIndex(up => up.UserId)
                   .IsUnique();

            builder.Property(up => up.PreferredCategories)
                   .HasMaxLength(500);

            builder.Property(up => up.TravelStyle)
                   .HasMaxLength(100);
        }
    }
}
