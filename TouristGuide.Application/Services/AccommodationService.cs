using TouristGuide.Application.DTOs.Stay;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class AccommodationService : IAccommodationService
    {
        private readonly IUnitOfWork _uow;

        public AccommodationService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<AccommodationDto>> SearchAsync(
            string? destination, DateTime? checkIn, DateTime? checkOut, int? guests, string? type)
        {
            var repo = _uow.GetRepository<Accommodation, int>();
            var all = await repo.GetAllAsync();
            var filtered = all.Where(a => a.IsActive && !a.IsDeleted);

            if (!string.IsNullOrEmpty(destination))
                filtered = filtered.Where(a =>
                    a.City.Contains(destination, StringComparison.OrdinalIgnoreCase) ||
                    a.Country.Contains(destination, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(type) && Enum.TryParse<AccommodationType>(type, true, out var at))
                filtered = filtered.Where(a => a.Type == at);

            return filtered.Select(MapToDto).ToList();
        }

        public async Task<AccommodationDto?> GetByIdAsync(int id)
        {
            var repo = _uow.GetRepository<Accommodation, int>();
            var a = await repo.GetByIdAsync(id);
            return a is null ? null : MapToDto(a);
        }

        public async Task<IReadOnlyList<RoomDto>> GetRoomsAsync(int accommodationId)
        {
            var repo = _uow.GetRepository<Room, int>();
            var all = await repo.GetAllAsync();
            return all.Where(r => r.AccommodationId == accommodationId && !r.IsDeleted)
                      .Select(r => new RoomDto
                      {
                          Id = r.Id, RoomNumber = r.RoomNumber, RoomType = r.RoomType.ToString(),
                          Description = r.Description, PricePerNight = r.PricePerNight,
                          MaxOccupancy = r.MaxOccupancy, BedCount = r.BedCount,
                          IsAvailable = r.IsAvailable, ImageUrl = r.ImageUrl
                      }).ToList();
        }

        public async Task<AccommodationDto> CreateAsync(AccommodationDto dto)
        {
            var repo = _uow.GetRepository<Accommodation, int>();
            var entity = new Accommodation
            {
                Name = dto.Name, Description = dto.Description, City = dto.City,
                Country = dto.Country, PricePerNight = dto.PricePerNight,
                Currency = dto.Currency, ImageUrl = dto.ImageUrl,
                ThumbnailUrl = dto.ThumbnailUrl, WebsiteUrl = dto.WebsiteUrl,
                Latitude = dto.Latitude, Longitude = dto.Longitude, IsActive = true,
                Type = Enum.TryParse<AccommodationType>(dto.Type, true, out var at)
                    ? at : AccommodationType.Hotel
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<AccommodationDto> UpdateAsync(int id, AccommodationDto dto)
        {
            var repo = _uow.GetRepository<Accommodation, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Accommodation {id} not found.");
            entity.Name = dto.Name; entity.Description = dto.Description;
            entity.City = dto.City; entity.Country = dto.Country;
            entity.PricePerNight = dto.PricePerNight;
            entity.UpdatedAt = DateTime.UtcNow;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        private static AccommodationDto MapToDto(Accommodation a) => new()
        {
            Id = a.Id, Name = a.Name, Description = a.Description,
            City = a.City, Country = a.Country, Type = a.Type.ToString(),
            StarRating = a.StarRating, PricePerNight = a.PricePerNight,
            Currency = a.Currency, AverageRating = a.AverageRating,
            ReviewCount = a.ReviewCount, ImageUrl = a.ImageUrl,
            ThumbnailUrl = a.ThumbnailUrl, WebsiteUrl = a.WebsiteUrl,
            Latitude = a.Latitude, Longitude = a.Longitude,
            Amenities = a.Amenities.Select(am => am.Name).ToList()
        };
    }
}
