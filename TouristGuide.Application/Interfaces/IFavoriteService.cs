using TouristGuide.Application.DTOs.Favorite;

namespace TouristGuide.Application.Interfaces
{
    public interface IFavoriteService
    {
        Task<IReadOnlyList<UserFavoriteDto>> GetAllAsync(string userId, string? entityType = null);
        Task<UserFavoriteDto> ToggleAsync(string userId, ToggleFavoriteDto dto);
        Task<bool> IsFavoriteAsync(string userId, string entityType, int entityId);
    }
}
