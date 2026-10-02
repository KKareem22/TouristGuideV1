using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Tour : BaseEntity<int>
    {
        public string Title { get; set; } = string.Empty;           // "The Amalfi Local Experience"
        public string Description { get; set; } = string.Empty;
        public string ExperienceType { get; set; } = string.Empty;  // "SMALL GROUP EXPERIENCE"
        public decimal PricePerPerson { get; set; }
        public int DurationInMinutes { get; set; }                   // shown as "4 hours"
        public int MaxGroupSize { get; set; }                        // "Max 8 guests"
        public string Languages { get; set; } = string.Empty;       // "English|Italian"
        public string? CoverImageUrl { get; set; }
        public decimal AverageRating { get; set; }                   // cached
        public int ReviewCount { get; set; }                         // cached
        public TourStatus Status { get; set; } = TourStatus.Draft;

        public int GuideProfileId { get; set; }
        public GuideProfile GuideProfile { get; set; } = null!;

        // Navigation
        public ICollection<TourItineraryStep> ItinerarySteps { get; set; } = new List<TourItineraryStep>();
        public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
    }
}
