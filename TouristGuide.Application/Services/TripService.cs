using TouristGuide.Application.DTOs.Trip;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class TripService : ITripService
    {
        private readonly IUnitOfWork _uow;

        public TripService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<TripDto>> GetForTouristAsync(string userId, bool upcoming)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var repo = _uow.GetRepository<Trip, int>();
            var all = await repo.GetAllAsync();
            var now = DateTime.UtcNow;
            var filtered = all.Where(t => t.TouristProfileId == profile.Id && !t.IsDeleted);
            filtered = upcoming
                ? filtered.Where(t => t.EndDate >= now)
                : filtered.Where(t => t.EndDate < now);

            return filtered.OrderBy(t => t.StartDate).Select(MapToDto).ToList();
        }

        public async Task<TripDto?> GetByIdAsync(int id)
        {
            var repo = _uow.GetRepository<Trip, int>();
            var trip = await repo.GetByIdAsync(id);
            return trip is null ? null : MapToDto(trip);
        }

        public async Task<TripDto> CreateAsync(string userId, TripDto dto)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var repo = _uow.GetRepository<Trip, int>();
            var entity = new Trip
            {
                Name = dto.Name, StartDate = dto.StartDate, EndDate = dto.EndDate,
                TravelerCount = dto.TravelerCount, CoverImageUrl = dto.CoverImageUrl,
                EstimatedBudgetPerPerson = dto.EstimatedBudgetPerPerson,
                IsAiGenerated = dto.IsAiGenerated,
                Status = TripStatus.Planned, TouristProfileId = profile.Id
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<TripDto> UpdateAsync(int id, string userId, TripDto dto)
        {
            var repo = _uow.GetRepository<Trip, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Trip {id} not found.");
            entity.Name = dto.Name; entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate; entity.TravelerCount = dto.TravelerCount;
            entity.CoverImageUrl = dto.CoverImageUrl;
            entity.EstimatedBudgetPerPerson = dto.EstimatedBudgetPerPerson;
            entity.UpdatedAt = DateTime.UtcNow;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var repo = _uow.GetRepository<Trip, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Trip {id} not found.");
            entity.IsDeleted = true;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
        }

        private static TripDto MapToDto(Trip t) => new()
        {
            Id = t.Id, Name = t.Name, StartDate = t.StartDate, EndDate = t.EndDate,
            Status = t.Status.ToString(), TravelerCount = t.TravelerCount,
            CoverImageUrl = t.CoverImageUrl, EstimatedBudgetPerPerson = t.EstimatedBudgetPerPerson,
            IsAiGenerated = t.IsAiGenerated
        };
    }
}
