using TouristGuide.Application.DTOs.Place;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class TouristPlaceService : ITouristPlaceService
    {
        private readonly IUnitOfWork _uow;

        public TouristPlaceService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<TouristPlaceDto>> GetAllAsync(string? category = null, bool? featured = null)
        {
            var repo = _uow.GetRepository<TouristPlace, int>();
            var all = await repo.GetAllAsync();

            var filtered = all.Where(p => !p.IsDeleted);
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<PlaceCategory>(category, true, out var cat))
                filtered = filtered.Where(p => p.Category == cat);
            if (featured.HasValue)
                filtered = filtered.Where(p => p.IsFeatured == featured.Value);

            return filtered.Select(MapToDto).ToList();
        }

        public async Task<TouristPlaceDto?> GetByIdAsync(int id)
        {
            var repo = _uow.GetRepository<TouristPlace, int>();
            var place = await repo.GetByIdAsync(id);
            return place is null ? null : MapToDto(place);
        }

        public async Task<IReadOnlyList<ActivityDto>> GetActivitiesAsync(int placeId)
        {
            var repo = _uow.GetRepository<Activity, int>();
            var all = await repo.GetAllAsync();
            return all.Where(a => a.TouristPlaceId == placeId && !a.IsDeleted)
                      .Select(a => new ActivityDto
                      {
                          Id = a.Id, Name = a.Name, Description = a.Description,
                          Price = a.Price, DurationInMinutes = a.DurationInMinutes,
                          Badge = a.Badge, TouristPlaceId = a.TouristPlaceId
                      }).ToList();
        }

        public async Task<TouristPlaceDto> CreateAsync(TouristPlaceDto dto)
        {
            var repo = _uow.GetRepository<TouristPlace, int>();
            var entity = new TouristPlace
            {
                Name = dto.Name, Description = dto.Description, City = dto.City,
                Country = dto.Country, Category = Enum.Parse<PlaceCategory>(dto.Category),
                EntryFee = dto.EntryFee, ImageUrl = dto.ImageUrl, ThumbnailUrl = dto.ThumbnailUrl,
                Tags = dto.Tags, IsFeatured = dto.IsFeatured,
                Latitude = dto.Latitude, Longitude = dto.Longitude
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<TouristPlaceDto> UpdateAsync(int id, TouristPlaceDto dto)
        {
            var repo = _uow.GetRepository<TouristPlace, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Place {id} not found.");
            entity.Name = dto.Name; entity.Description = dto.Description;
            entity.City = dto.City; entity.Country = dto.Country;
            entity.Tags = dto.Tags; entity.IsFeatured = dto.IsFeatured;
            entity.UpdatedAt = DateTime.UtcNow;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var repo = _uow.GetRepository<TouristPlace, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Place {id} not found.");
            entity.IsDeleted = true;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
        }

        private static TouristPlaceDto MapToDto(TouristPlace p) => new()
        {
            Id = p.Id, Name = p.Name, Description = p.Description, City = p.City,
            Country = p.Country, Category = p.Category.ToString(), EntryFee = p.EntryFee,
            ImageUrl = p.ImageUrl, ThumbnailUrl = p.ThumbnailUrl, Tags = p.Tags,
            AverageRating = p.AverageRating, ReviewCount = p.ReviewCount,
            IsFeatured = p.IsFeatured, Latitude = p.Latitude, Longitude = p.Longitude
        };
    }
}
