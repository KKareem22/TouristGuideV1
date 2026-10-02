using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class GuideWeeklyAvailability : BaseEntity<int>
    {
        public DayOfWeek Day { get; set; }       // Monday=1 … Sunday=0 (System.DayOfWeek)
        public bool IsAvailable { get; set; } = true;

        public int GuideProfileId { get; set; }
        public GuideProfile GuideProfile { get; set; } = null!;
    }
}
