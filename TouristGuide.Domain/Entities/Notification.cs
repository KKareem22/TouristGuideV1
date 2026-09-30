using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class Notification : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
    }
}
