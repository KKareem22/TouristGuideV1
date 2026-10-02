using TouristGuide.Application.DTOs.Guide;

namespace TouristGuide.Application.Interfaces
{
    public interface ITourBookingService
    {
        Task<IReadOnlyList<TourBookingDto>> GetForTouristAsync(string userId);
        Task<IReadOnlyList<TourBookingDto>> GetRequestsForGuideAsync(string userId);
        Task<TourBookingDto> CreateAsync(string userId, CreateTourBookingDto dto);
        Task<TourBookingDto> AcceptAsync(int bookingId, string guideUserId);
        Task<TourBookingDto> DeclineAsync(int bookingId, string guideUserId);
    }
}
