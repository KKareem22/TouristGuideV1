using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class UserSettings : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;       // FK → ApplicationUser (1-to-1)
        public bool PushNotificationsEnabled { get; set; } = true;
        public bool DarkModeEnabled { get; set; } = false;
        public string Language { get; set; } = "English";
        public string Currency { get; set; } = "USD";
    }
}
