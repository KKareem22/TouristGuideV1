using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class AiChatMessage : BaseEntity<int>
    {
        public AiMessageRole Role { get; set; }              // User | Assistant
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int? TokensUsed { get; set; }                 // for cost tracking

        public int SessionId { get; set; }
        public AiChatSession Session { get; set; } = null!;
    }
}
