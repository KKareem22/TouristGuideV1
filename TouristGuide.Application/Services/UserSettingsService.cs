using TouristGuide.Application.DTOs.Settings;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class UserSettingsService : IUserSettingsService
    {
        private readonly IUnitOfWork _uow;

        public UserSettingsService(IUnitOfWork uow) => _uow = uow;

        public async Task<UserSettingsDto> GetAsync(string userId)
        {
            var repo = _uow.GetRepository<UserSettings, int>();
            var all = await repo.GetAllAsync();
            var settings = all.FirstOrDefault(s => s.UserId == userId);
            if (settings is null)
            {
                // Auto-create defaults on first access
                settings = new UserSettings { UserId = userId };
                repo.Add(settings);
                await _uow.SaveChangesAsync();
            }
            return MapToDto(settings);
        }

        public async Task<UserSettingsDto> UpdateAsync(string userId, UserSettingsDto dto)
        {
            var repo = _uow.GetRepository<UserSettings, int>();
            var all = await repo.GetAllAsync();
            var settings = all.FirstOrDefault(s => s.UserId == userId);
            if (settings is null)
            {
                settings = new UserSettings { UserId = userId };
                repo.Add(settings);
            }
            settings.PushNotificationsEnabled = dto.PushNotificationsEnabled;
            settings.DarkModeEnabled = dto.DarkModeEnabled;
            settings.Language = dto.Language;
            settings.Currency = dto.Currency;
            settings.UpdatedAt = DateTime.UtcNow;
            repo.Update(settings);
            await _uow.SaveChangesAsync();
            return MapToDto(settings);
        }

        private static UserSettingsDto MapToDto(UserSettings s) => new()
        {
            PushNotificationsEnabled = s.PushNotificationsEnabled,
            DarkModeEnabled = s.DarkModeEnabled,
            Language = s.Language,
            Currency = s.Currency
        };
    }
}
