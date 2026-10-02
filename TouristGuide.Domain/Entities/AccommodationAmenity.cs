using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class AccommodationAmenity : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;     // "Free WiFi", "Pool"
        public string? IconCode { get; set; }                // for mobile icon rendering

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;
    }
}
