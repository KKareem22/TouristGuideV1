namespace TouristGuide.Application.DTOs.Trip
{
    public class TripDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TravelerCount { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? EstimatedBudgetPerPerson { get; set; }
        public bool IsAiGenerated { get; set; }
        public List<TripDayDto> TripDays { get; set; } = new();
    }

    public class TripDayDto
    {
        public int Id { get; set; }
        public int DayNumber { get; set; }
        public DateTime Date { get; set; }
        public string? DayTitle { get; set; }
        public List<ItineraryItemDto> ItineraryItems { get; set; } = new();
    }

    public class ItineraryItemDto
    {
        public int Id { get; set; }
        public string StartTime { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? IconType { get; set; }
        public string ItemType { get; set; } = string.Empty;
    }
}
