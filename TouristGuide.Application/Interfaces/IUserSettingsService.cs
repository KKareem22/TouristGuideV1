using TouristGuide.Application.DTOs.Settings;

namespace TouristGuide.Application.Interfaces
{
    public interface IUserSettingsService
    {
        Task<UserSettingsDto> GetAsync(string userId);
        Task<UserSettingsDto> UpdateAsync(string userId, UserSettingsDto dto);
    }
}
