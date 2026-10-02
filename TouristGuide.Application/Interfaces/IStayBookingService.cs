using TouristGuide.Application.DTOs.Stay;

namespace TouristGuide.Application.Interfaces
{
    public interface IStayBookingService
    {
        Task<IReadOnlyList<StayBookingDto>> GetForTouristAsync(string userId);
        Task<StayBookingDto> CreateAsync(string userId, CreateStayBookingDto dto);
        Task<StayBookingDto> CancelAsync(int bookingId, string userId);
    }
}
