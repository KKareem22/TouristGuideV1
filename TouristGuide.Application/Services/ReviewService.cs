using TouristGuide.Application.DTOs.Review;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _uow;

        public ReviewService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<ReviewDto>> GetForEntityAsync(string referenceType, int referenceId)
        {
            if (!Enum.TryParse<ReferenceType>(referenceType, true, out var rt))
                throw new Exception($"Invalid reference type: {referenceType}");

            var repo = _uow.GetRepository<Review, int>();
            var all = await repo.GetAllAsync();
            return all.Where(r => r.ReferenceType == rt && r.ReferenceId == referenceId && !r.IsDeleted)
                      .OrderByDescending(r => r.ReviewDate)
                      .Select(r => new ReviewDto
                      {
                          Id = r.Id, ReferenceType = r.ReferenceType.ToString(),
                          ReferenceId = r.ReferenceId, Rating = r.Rating,
                          Comment = r.Comment, ReviewDate = r.ReviewDate
                      }).ToList();
        }

        public async Task<ReviewDto> CreateAsync(string userId, CreateReviewDto dto)
        {
            if (!Enum.TryParse<ReferenceType>(dto.ReferenceType, true, out var rt))
                throw new Exception($"Invalid reference type: {dto.ReferenceType}");

            // Resolve TouristProfileId from UserId
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var repo = _uow.GetRepository<Review, int>();
            var entity = new Review
            {
                ReferenceType = rt, ReferenceId = dto.ReferenceId,
                Rating = dto.Rating, Comment = dto.Comment,
                ReviewDate = DateTime.UtcNow, TouristProfileId = profile.Id
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return new ReviewDto
            {
                Id = entity.Id, ReferenceType = entity.ReferenceType.ToString(),
                ReferenceId = entity.ReferenceId, Rating = entity.Rating,
                Comment = entity.Comment, ReviewDate = entity.ReviewDate
            };
        }

        public async Task DeleteAsync(int id, string userId)
        {
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new UnauthorizedAccessException();

            var repo = _uow.GetRepository<Review, int>();
            var review = await repo.GetByIdAsync(id) ?? throw new Exception($"Review {id} not found.");
            if (review.TouristProfileId != profile.Id) throw new UnauthorizedAccessException();
            review.IsDeleted = true;
            repo.Update(review);
            await _uow.SaveChangesAsync();
        }
    }
}
