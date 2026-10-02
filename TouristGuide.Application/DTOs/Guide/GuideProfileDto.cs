namespace TouristGuide.Application.DTOs.Guide
{
    public class GuideProfileDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Languages { get; set; } = string.Empty;
        public string Specialties { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public int ExperienceYears { get; set; }
        public int HappyTravelers { get; set; }
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public decimal PricePerPerson { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
    }
}
