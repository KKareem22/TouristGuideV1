using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class TouristPlace : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string? LocationUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public PlaceCategory Category { get; set; }
        public decimal EntryFee { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string? Tags { get; set; }                // "Coastal escape|City break|Island retreat"
        public decimal AverageRating { get; set; }       // cached
        public int ReviewCount { get; set; }             // cached
        public bool IsFeatured { get; set; } = false;

        // Navigation
        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
