namespace TouristGuide.Application.DTOs.Guide
{
    public class TourBookingDto
    {
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public int TravelerCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
        public int TouristProfileId { get; set; }
        public int TourId { get; set; }
        public string TourTitle { get; set; } = string.Empty;
    }

    public class CreateTourBookingDto
    {
        public DateTime BookingDate { get; set; }
        public int TravelerCount { get; set; }
        public string? SpecialRequests { get; set; }
        public int TourId { get; set; }
    }
}
