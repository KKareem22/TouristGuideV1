using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class TourService : ITourService
    {
        private readonly IUnitOfWork _uow;

        public TourService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<TourDto>> GetAllAsync(int? guideProfileId = null, string? status = null)
        {
            var repo = _uow.GetRepository<Tour, int>();
            var all = await repo.GetAllAsync();
            var filtered = all.Where(t => !t.IsDeleted);
            if (guideProfileId.HasValue)
                filtered = filtered.Where(t => t.GuideProfileId == guideProfileId.Value);
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<TourStatus>(status, true, out var ts))
                filtered = filtered.Where(t => t.Status == ts);
            return filtered.Select(MapToDto).ToList();
        }

        public async Task<TourDto?> GetByIdAsync(int id)
        {
            var repo = _uow.GetRepository<Tour, int>();
            var t = await repo.GetByIdAsync(id);
            return t is null ? null : MapToDto(t);
        }

        public async Task<TourDto> CreateAsync(string userId, TourDto dto)
        {
            var guideRepo = _uow.GetRepository<GuideProfile, int>();
            var guides = await guideRepo.GetAllAsync();
            var guide = guides.FirstOrDefault(g => g.UserId == userId && !g.IsDeleted)
                ?? throw new Exception("Guide profile not found.");

            var repo = _uow.GetRepository<Tour, int>();
            var entity = new Tour
            {
                Title = dto.Title, Description = dto.Description,
                ExperienceType = dto.ExperienceType, PricePerPerson = dto.PricePerPerson,
                DurationInMinutes = dto.DurationInMinutes, MaxGroupSize = dto.MaxGroupSize,
                Languages = dto.Languages, CoverImageUrl = dto.CoverImageUrl,
                Status = TourStatus.Draft, GuideProfileId = guide.Id
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task<TourDto> UpdateAsync(int id, string userId, TourDto dto)
        {
            var repo = _uow.GetRepository<Tour, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Tour {id} not found.");
            entity.Title = dto.Title; entity.Description = dto.Description;
            entity.PricePerPerson = dto.PricePerPerson;
            entity.DurationInMinutes = dto.DurationInMinutes;
            entity.MaxGroupSize = dto.MaxGroupSize; entity.Languages = dto.Languages;
            entity.CoverImageUrl = dto.CoverImageUrl;
            if (Enum.TryParse<TourStatus>(dto.Status, true, out var ts)) entity.Status = ts;
            entity.UpdatedAt = DateTime.UtcNow;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var repo = _uow.GetRepository<Tour, int>();
            var entity = await repo.GetByIdAsync(id) ?? throw new Exception($"Tour {id} not found.");
            entity.IsDeleted = true;
            repo.Update(entity);
            await _uow.SaveChangesAsync();
        }

        private static TourDto MapToDto(Tour t) => new()
        {
            Id = t.Id, Title = t.Title, Description = t.Description,
            ExperienceType = t.ExperienceType, PricePerPerson = t.PricePerPerson,
            DurationInMinutes = t.DurationInMinutes, MaxGroupSize = t.MaxGroupSize,
            Languages = t.Languages, CoverImageUrl = t.CoverImageUrl,
            AverageRating = t.AverageRating, ReviewCount = t.ReviewCount,
            Status = t.Status.ToString(), GuideProfileId = t.GuideProfileId,
            ItinerarySteps = t.ItinerarySteps.Select(s => new TourItineraryStepDto
            {
                StepNumber = s.StepNumber, Title = s.Title, Description = s.Description
            }).ToList()
        };
    }
}
