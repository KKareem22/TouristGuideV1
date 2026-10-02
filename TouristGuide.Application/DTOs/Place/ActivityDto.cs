namespace TouristGuide.Application.DTOs.Place
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }
        public string? Badge { get; set; }
        public int TouristPlaceId { get; set; }
    }
}
