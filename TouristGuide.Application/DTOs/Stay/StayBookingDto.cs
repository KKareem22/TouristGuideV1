namespace TouristGuide.Application.DTOs.Stay
{
    public class StayBookingDto
    {
        public int Id { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int TotalNights { get; set; }
        public int GuestCount { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? SpecialRequests { get; set; }
        public int AccommodationId { get; set; }
        public string AccommodationName { get; set; } = string.Empty;
        public int RoomId { get; set; }
    }

    public class CreateStayBookingDto
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int GuestCount { get; set; }
        public string? SpecialRequests { get; set; }
        public int RoomId { get; set; }
        public int AccommodationId { get; set; }
    }
}
