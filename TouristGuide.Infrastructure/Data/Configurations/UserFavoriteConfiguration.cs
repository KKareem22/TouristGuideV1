using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data.Configurations
{
    public class UserFavoriteConfiguration : IEntityTypeConfiguration<UserFavorite>
    {
        public void Configure(EntityTypeBuilder<UserFavorite> builder)
        {
            // Prevent same user saving the same entity twice
            builder.HasIndex(uf => new { uf.UserId, uf.EntityType, uf.EntityId })
                   .IsUnique();

            builder.HasOne(uf => uf.TouristProfile)
                   .WithMany(t => t.Favorites)
                   .HasForeignKey(uf => uf.TouristProfileId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
