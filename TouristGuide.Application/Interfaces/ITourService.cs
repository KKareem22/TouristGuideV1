using TouristGuide.Application.DTOs.Guide;

namespace TouristGuide.Application.Interfaces
{
    public interface ITourService
    {
        Task<IReadOnlyList<TourDto>> GetAllAsync(int? guideProfileId = null, string? status = null);
        Task<TourDto?> GetByIdAsync(int id);
        Task<TourDto> CreateAsync(string userId, TourDto dto);
        Task<TourDto> UpdateAsync(int id, string userId, TourDto dto);
        Task DeleteAsync(int id, string userId);
    }
}
