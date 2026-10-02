using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class AiRecommendationLog : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public AiRecommendationType RecommendationType { get; set; }
        public int? RecommendedEntityId { get; set; }
        public decimal Score { get; set; }                    // confidence/relevance score
        public bool? WasAccepted { get; set; }               // did user act on it?
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
        public string? Feedback { get; set; }
    }
}
