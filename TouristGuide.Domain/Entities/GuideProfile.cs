using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class GuideProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }
        public bool IsVerified { get; set; } = false;

        public ICollection<GuideAvailability> Availabilities { get; set; } = new List<GuideAvailability>();
        public ICollection<GuideBooking> Bookings { get; set; } = new List<GuideBooking>();
    }
}
