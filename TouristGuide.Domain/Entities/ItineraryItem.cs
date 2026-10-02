using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class ItineraryItem : BaseEntity<int>
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Title { get; set; } = string.Empty;        // "Arrive in Naples"
        public string? Description { get; set; }                  // "Transfer to your hotel..."
        public decimal? EstimatedCost { get; set; }               // "$65 estimated"
        public string? IconType { get; set; }                     // "car" | "fork" | "compass"
        public ItemType ItemType { get; set; }
        public int? ReferenceId { get; set; }                     // FK to Place or Activity

        public int TripDayId { get; set; }
        public TripDay TripDay { get; set; } = null!;
    }
}
