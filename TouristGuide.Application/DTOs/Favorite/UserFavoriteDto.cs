namespace TouristGuide.Application.DTOs.Favorite
{
    public class UserFavoriteDto
    {
        public int Id { get; set; }
        public string EntityType { get; set; } = string.Empty;   // Place | Guide | Accommodation
        public int EntityId { get; set; }
        public DateTime SavedAt { get; set; }
    }

    public class ToggleFavoriteDto
    {
        public string EntityType { get; set; } = string.Empty;
        public int EntityId { get; set; }
    }
}
