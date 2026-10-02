using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class GuideProfile : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;     // "Positano, Italy"
        public string Languages { get; set; } = string.Empty;    // pipe-separated: "English|Italian"
        public string Specialties { get; set; } = string.Empty;  // "Food & culture|Hidden gems"
        public string? LicenseNumber { get; set; }
        public bool IsVerified { get; set; } = false;
        public int ExperienceYears { get; set; }
        public int HappyTravelers { get; set; }                  // cached counter
        public decimal AverageRating { get; set; }               // cached
        public int ReviewCount { get; set; }                     // cached
        public decimal PricePerPerson { get; set; }              // "From $85/person"
        public string? ProfileImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? PhoneNumber { get; set; }

        // Navigation
        public ICollection<Tour> Tours { get; set; } = new List<Tour>();
        public ICollection<GuideWeeklyAvailability> WeeklyAvailabilities { get; set; } = new List<GuideWeeklyAvailability>();
        public ICollection<TourBooking> TourBookings { get; set; } = new List<TourBooking>();
    }
}
