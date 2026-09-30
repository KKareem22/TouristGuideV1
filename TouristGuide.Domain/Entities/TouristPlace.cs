using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class TouristPlace : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string LocationUrl { get; set; } = string.Empty;
        public Category Category { get; set; }
        public decimal EntryFee { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}
