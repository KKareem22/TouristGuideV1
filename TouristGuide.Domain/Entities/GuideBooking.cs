using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class GuideBooking : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;

        public int GuideProfileId { get; set; }
        public GuideProfile GuideProfile { get; set; } = null!;
    }
}
