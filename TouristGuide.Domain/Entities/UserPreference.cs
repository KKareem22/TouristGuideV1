using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class UserPreference : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;        // FK → ApplicationUser (1-to-1)
        public string? PreferredCategories { get; set; }           // "Food & drink|Nature"
        public string? PreferredBudgetTier { get; set; }           // "Comfort"
        public string? TravelStyle { get; set; }                   // "Adventure"
        public TravelGroupType TravelGroupType { get; set; } = TravelGroupType.Solo;
        public int MaxDailyActivities { get; set; } = 3;
    }
}
