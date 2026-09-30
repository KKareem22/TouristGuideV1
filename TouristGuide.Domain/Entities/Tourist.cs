using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class Tourist : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public string PreferredLanguage { get; set; } = string.Empty;
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<GuideBooking> GuideBookings { get; set; } = new List<GuideBooking>();
        public ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
