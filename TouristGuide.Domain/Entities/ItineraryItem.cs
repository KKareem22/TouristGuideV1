using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class ItineraryItem : BaseEntity<int>
    {
        public ItemType ItemType { get; set; }
        public int? ReferenceId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int TripDayId { get; set; }
        public TripDay TripDay { get; set; } = null!;
    }
}
