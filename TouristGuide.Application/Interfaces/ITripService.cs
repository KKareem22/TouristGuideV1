using TouristGuide.Application.DTOs.Trip;

namespace TouristGuide.Application.Interfaces
{
    public interface ITripService
    {
        Task<IReadOnlyList<TripDto>> GetForTouristAsync(string userId, bool upcoming);
        Task<TripDto?> GetByIdAsync(int id);
        Task<TripDto> CreateAsync(string userId, TripDto dto);
        Task<TripDto> UpdateAsync(int id, string userId, TripDto dto);
        Task DeleteAsync(int id, string userId);
    }
}
