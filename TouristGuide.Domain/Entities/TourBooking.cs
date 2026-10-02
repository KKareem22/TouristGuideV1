using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class TourBooking : BaseEntity<int>
    {
        public DateTime BookingDate { get; set; }
        public int TravelerCount { get; set; }               // "2 travelers"
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public string? SpecialRequests { get; set; }

        public int TouristProfileId { get; set; }
        public TouristProfile TouristProfile { get; set; } = null!;

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
    }
}
