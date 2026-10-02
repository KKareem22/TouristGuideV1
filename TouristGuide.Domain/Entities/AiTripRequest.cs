using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class AiTripRequest : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;      // FK → ApplicationUser
        public string Destination { get; set; } = string.Empty; // "Amalfi Coast, Italy"
        public DateTime TravelDate { get; set; }
        public int TravelerCount { get; set; }
        public BudgetTier BudgetTier { get; set; }
        public string Interests { get; set; } = string.Empty;   // "Food & drink|Nature|Culture"
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        public bool WasConverted { get; set; } = false;          // true when user saved the trip
        public int? GeneratedTripId { get; set; }                // FK → Trip (nullable)
    }
}
