using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class StayBooking : BaseEntity<int>
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialRequests { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int TouristProfileId { get; set; }
        public TouristProfile TouristProfile { get; set; } = null!;

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;
    }
}
