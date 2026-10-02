using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class TripDay : BaseEntity<int>
    {
        public int DayNumber { get; set; }
        public DateTime Date { get; set; }
        public string? DayTitle { get; set; }   // "Arrive & unwind"

        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        // Navigation
        public ICollection<ItineraryItem> ItineraryItems { get; set; } = new List<ItineraryItem>();
    }
}
