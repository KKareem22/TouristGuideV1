namespace TouristGuide.Application.DTOs.Guide
{
    public class TourDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ExperienceType { get; set; } = string.Empty;
        public decimal PricePerPerson { get; set; }
        public int DurationInMinutes { get; set; }
        public int MaxGroupSize { get; set; }
        public string Languages { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int GuideProfileId { get; set; }
        public List<TourItineraryStepDto> ItinerarySteps { get; set; } = new();
    }

    public class TourItineraryStepDto
    {
        public int StepNumber { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
