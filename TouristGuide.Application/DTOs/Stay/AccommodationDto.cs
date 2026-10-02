namespace TouristGuide.Application.DTOs.Stay
{
    public class AccommodationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public decimal PricePerNight { get; set; }
        public string Currency { get; set; } = "USD";
        public decimal AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public List<string> Amenities { get; set; } = new();
        public List<RoomDto> Rooms { get; set; } = new();
    }
}
