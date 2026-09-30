using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class ServiceBooking: BaseEntity<int>
    {
        public DateTime BookingDate { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
    }
}