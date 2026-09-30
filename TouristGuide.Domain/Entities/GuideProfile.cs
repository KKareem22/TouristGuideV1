using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class GuideProfile : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public string LicenseNumber { get; set; } = string.Empty;
        public bool IsVerified { get; set; } = false;

        public int ExperienceYears { get; set; }
        public decimal PricePerDay { get; set; }
        public string Languages { get; set; } = string.Empty;

        // العلاقات الشاملة
        public ICollection<GuideAvailability> Availabilities { get; set; } = new List<GuideAvailability>();
        public ICollection<GuideBooking> Bookings { get; set; } = new List<GuideBooking>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
