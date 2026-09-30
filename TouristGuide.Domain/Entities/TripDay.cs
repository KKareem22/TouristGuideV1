using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class TripDay : BaseEntity<int>
    {
        public DateTime Date { get; set; }
        public int DayNumber { get; set; }

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public ICollection<ItineraryItem> ItineraryItems { get; set; } = new List<ItineraryItem>();
    }
}
