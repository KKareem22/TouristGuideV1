using TouristGuide.Application.DTOs.Review;

namespace TouristGuide.Application.Interfaces
{
    public interface IReviewService
    {
        Task<IReadOnlyList<ReviewDto>> GetForEntityAsync(string referenceType, int referenceId);
        Task<ReviewDto> CreateAsync(string userId, CreateReviewDto dto);
        Task DeleteAsync(int id, string userId);
    }
}
