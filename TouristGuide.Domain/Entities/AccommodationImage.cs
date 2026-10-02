using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class AccommodationImage : BaseEntity<int>
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }

        public int AccommodationId { get; set; }
        public Accommodation Accommodation { get; set; } = null!;
    }
}
