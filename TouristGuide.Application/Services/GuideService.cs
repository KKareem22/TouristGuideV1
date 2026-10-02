using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class GuideService : IGuideService
    {
        private readonly IUnitOfWork _uow;

        public GuideService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<GuideProfileDto>> GetAllAsync(string? language = null, string? specialty = null)
        {
            var repo = _uow.GetRepository<GuideProfile, int>();
            var all = await repo.GetAllAsync();
            var filtered = all.Where(g => !g.IsDeleted);
            if (!string.IsNullOrEmpty(language))
                filtered = filtered.Where(g => g.Languages.Contains(language, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(specialty))
                filtered = filtered.Where(g => g.Specialties.Contains(specialty, StringComparison.OrdinalIgnoreCase));
            return filtered.Select(MapToDto).ToList();
        }

        public async Task<GuideProfileDto?> GetByIdAsync(int id)
        {
            var repo = _uow.GetRepository<GuideProfile, int>();
            var g = await repo.GetByIdAsync(id);
            return g is null ? null : MapToDto(g);
        }

        public async Task<GuideProfileDto?> GetByUserIdAsync(string userId)
        {
            var repo = _uow.GetRepository<GuideProfile, int>();
            var all = await repo.GetAllAsync();
            var g = all.FirstOrDefault(x => x.UserId == userId && !x.IsDeleted);
            return g is null ? null : MapToDto(g);
        }

        public async Task<GuideProfileDto> UpdateAsync(string userId, GuideProfileDto dto)
        {
            var repo = _uow.GetRepository<GuideProfile, int>();
            var all = await repo.GetAllAsync();
            var entity = all.FirstOrDefault(g => g.UserId == userId && !g.IsDeleted)
                ?? throw new Exception("Guide profile not found.");
            entity.Bio = dto.Bio; entity.Location = dto.Location;
            entity.Languages = dto.Languages; entity.Specialties = dto.Specialties;
            entity.PricePerPerson = dto.PricePerPerson;
            entity.ProfileImageUrl = dto.ProfileImageUrl;
            entity.CoverImageUrl = dto.CoverImageUrl;
            entity.UpdatedAt = DateTime.UtcNow;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task SetWeeklyAvailabilityAsync(string userId, Dictionary<DayOfWeek, bool> schedule)
        {
            var guideRepo = _uow.GetRepository<GuideProfile, int>();
            var allGuides = await guideRepo.GetAllAsync();
            var guide = allGuides.FirstOrDefault(g => g.UserId == userId && !g.IsDeleted)
                ?? throw new Exception("Guide profile not found.");

            var availRepo = _uow.GetRepository<GuideWeeklyAvailability, int>();
            var existing = await availRepo.GetAllAsync();
            var guideAvail = existing.Where(a => a.GuideProfileId == guide.Id).ToList();

            foreach (var entry in schedule)
            {
                var record = guideAvail.FirstOrDefault(a => a.Day == entry.Key);
                if (record is null)
                {
                    availRepo.Add(new GuideWeeklyAvailability { GuideProfileId = guide.Id, Day = entry.Key, IsAvailable = entry.Value });
                }
                else
                {
                    record.IsAvailable = entry.Value;
                    record.UpdatedAt = DateTime.UtcNow;
                    availRepo.Update(record);
                }
            }
            await _uow.SaveChangesAsync();
        }

        public async Task<Dictionary<DayOfWeek, bool>> GetWeeklyAvailabilityAsync(int guideId)
        {
            var repo = _uow.GetRepository<GuideWeeklyAvailability, int>();
            var all = await repo.GetAllAsync();
            return all.Where(a => a.GuideProfileId == guideId)
                      .ToDictionary(a => a.Day, a => a.IsAvailable);
        }

        private static GuideProfileDto MapToDto(GuideProfile g) => new()
        {
            Id = g.Id, UserId = g.UserId, Bio = g.Bio, Location = g.Location,
            Languages = g.Languages, Specialties = g.Specialties,
            IsVerified = g.IsVerified, ExperienceYears = g.ExperienceYears,
            HappyTravelers = g.HappyTravelers, AverageRating = g.AverageRating,
            ReviewCount = g.ReviewCount, PricePerPerson = g.PricePerPerson,
            ProfileImageUrl = g.ProfileImageUrl, CoverImageUrl = g.CoverImageUrl
        };
    }
}
