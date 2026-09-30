using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class GuideAvailability : BaseEntity<int>
    {
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; } = true;

        public int GuideProfileId { get; set; }
        public GuideProfile GuideProfile { get; set; } = null!;
    }
}
