using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class TouristProfile : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string? DisplayLocation { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Nationality { get; set; }
        public string? PreferredLanguage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }

        // Navigation
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
        public ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();
        public ICollection<StayBooking> StayBookings { get; set; } = new List<StayBooking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
    }
}
