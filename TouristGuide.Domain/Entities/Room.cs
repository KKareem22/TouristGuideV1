using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Room : BaseEntity<int>
    {
        public string RoomNumber { get; set; } = string.Empty;
        public RoomType RoomType { get; set; }
        public string? Description { get; set; }
        public decimal PricePerNight { get; set; }
        public int MaxOccupancy { get; set; }
        public int BedCount { get; set; }
        public bool IsAvailable { get; set; } = true;
        public string? ImageUrl { get; set; }

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;

        // Navigation
        public ICollection<StayBooking> StayBookings { get; set; } = new List<StayBooking>();
    }
}
