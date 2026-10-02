namespace TouristGuide.Application.DTOs.AI
{
    public class UserPreferenceDto
    {
        public string? PreferredCategories { get; set; }
        public string? PreferredBudgetTier { get; set; }
        public string? TravelStyle { get; set; }
        public string TravelGroupType { get; set; } = "Solo";
        public int MaxDailyActivities { get; set; } = 3;
    }
}
