using TouristGuide.Application.DTOs.Guide;

namespace TouristGuide.Application.Interfaces
{
    public interface IGuideService
    {
        Task<IReadOnlyList<GuideProfileDto>> GetAllAsync(string? language = null, string? specialty = null);
        Task<GuideProfileDto?> GetByIdAsync(int id);
        Task<GuideProfileDto?> GetByUserIdAsync(string userId);
        Task<GuideProfileDto> UpdateAsync(string userId, GuideProfileDto dto);
        Task SetWeeklyAvailabilityAsync(string userId, Dictionary<DayOfWeek, bool> schedule);
        Task<Dictionary<DayOfWeek, bool>> GetWeeklyAvailabilityAsync(int guideId);
    }
}
