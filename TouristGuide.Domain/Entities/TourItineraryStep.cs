using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class TourItineraryStep : BaseEntity<int>
    {
        public int StepNumber { get; set; }                   // 01, 02, 03...
        public string Title { get; set; } = string.Empty;    // "Meet in Positano"
        public string Description { get; set; } = string.Empty;

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
    }
}
