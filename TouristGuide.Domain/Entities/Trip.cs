using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Trip : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TripStatus Status { get; set; }

        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;

        public ICollection<TripDay> TripDays { get; set; } = new List<TripDay>();
    }
}
