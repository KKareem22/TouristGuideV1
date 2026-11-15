using TouristGuide.Application.Common;
using TouristGuide.Application.DTOs.Place;

namespace TouristGuide.Application.Interfaces
{
    public interface ITouristPlaceService
    {
        Task<PagedResult<TouristPlaceDto>> GetAllAsync(PlaceSpecParams specParams); Task<TouristPlaceDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<ActivityDto>> GetActivitiesAsync(int placeId);
        Task<TouristPlaceDto> CreateAsync(TouristPlaceDto dto);
        Task<TouristPlaceDto> UpdateAsync(int id, TouristPlaceDto dto);
        Task DeleteAsync(int id);
    }
}
