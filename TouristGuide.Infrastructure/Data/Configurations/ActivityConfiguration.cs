using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.Property(a => a.Price)
                   .HasColumnType("decimal(18,2)");

            builder.Property(a => a.Name)
                   .HasMaxLength(150)
                   .IsRequired();
        }
    }
}
