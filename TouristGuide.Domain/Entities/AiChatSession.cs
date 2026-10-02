using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class AiChatSession : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;   // FK → ApplicationUser
        public string? Title { get; set; }                   // "Planning Cairo trip"
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<AiChatMessage> Messages { get; set; } = new List<AiChatMessage>();
    }
}
