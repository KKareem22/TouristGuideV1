using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Accommodation : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;         // "Canaves Oia Epitome"
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;         // "Oia"
        public string Country { get; set; } = string.Empty;      // "Santorini"
        public AccommodationType Type { get; set; }
        public int StarRating { get; set; }                       // 1–5
        public decimal PricePerNight { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal AverageRating { get; set; }                // cached
        public int ReviewCount { get; set; }                      // cached
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? WebsiteUrl { get; set; }
        public TimeSpan CheckInTime { get; set; }
        public TimeSpan CheckOutTime { get; set; }
        public bool IsActive { get; set; } = true;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // Navigation
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
        public ICollection<AccommodationAmenity> Amenities { get; set; } = new List<AccommodationAmenity>();
        public ICollection<AccommodationImage> Images { get; set; } = new List<AccommodationImage>();
        public ICollection<StayBooking> StayBookings { get; set; } = new List<StayBooking>();
    }
}
