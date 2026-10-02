using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class UserFavorite : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;    // FK → ApplicationUser
        public FavoriteType EntityType { get; set; }           // Place | Guide | Accommodation
        public int EntityId { get; set; }                      // FK to respective entity
        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        public int TouristProfileId { get; set; }
        public TouristProfile TouristProfile { get; set; } = null!;
    }
}
