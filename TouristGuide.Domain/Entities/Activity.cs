using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class Activity : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int DurationInMinutes { get; set; }

        public int TouristPlaceId { get; set; }
        public TouristPlace TouristPlace { get; set; } = null!;
    }
}
