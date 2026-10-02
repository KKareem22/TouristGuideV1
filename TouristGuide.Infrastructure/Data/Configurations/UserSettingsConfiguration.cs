using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            // One settings record per user
            builder.HasIndex(us => us.UserId)
                   .IsUnique();

            builder.Property(us => us.Language)
                   .HasMaxLength(50)
                   .HasDefaultValue("English");

            builder.Property(us => us.Currency)
                   .HasMaxLength(10)
                   .HasDefaultValue("USD");
        }
    }
}
