namespace TouristGuide.Application.DTOs.AI
{
    public class AiTripRequestDto
    {
        public string Destination { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public int TravelerCount { get; set; }
        public string BudgetTier { get; set; } = "Comfort";     // Easygoing | Comfort | Luxury
        public List<string> Interests { get; set; } = new();    // ["Food & drink","Nature"]
    }

    public class GeneratedItineraryDto
    {
        public string Destination { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TravelerCount { get; set; }
        public string EstimatedBudgetPerPerson { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public List<GeneratedDayDto> Days { get; set; } = new();
    }

    public class GeneratedDayDto
    {
        public int DayNumber { get; set; }
        public DateTime Date { get; set; }
        public string? DayTitle { get; set; }
        public List<GeneratedItemDto> Items { get; set; } = new();
    }

    public class GeneratedItemDto
    {
        public string Time { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? IconType { get; set; }
    }
}
