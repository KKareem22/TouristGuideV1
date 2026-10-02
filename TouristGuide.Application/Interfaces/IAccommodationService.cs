using TouristGuide.Application.DTOs.Stay;

namespace TouristGuide.Application.Interfaces
{
    public interface IAccommodationService
    {
        Task<IReadOnlyList<AccommodationDto>> SearchAsync(string? destination, DateTime? checkIn, DateTime? checkOut, int? guests, string? type);
        Task<AccommodationDto?> GetByIdAsync(int id);
        Task<IReadOnlyList<RoomDto>> GetRoomsAsync(int accommodationId);
        Task<AccommodationDto> CreateAsync(AccommodationDto dto);
        Task<AccommodationDto> UpdateAsync(int id, AccommodationDto dto);
    }
}
