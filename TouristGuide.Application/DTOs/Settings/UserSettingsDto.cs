namespace TouristGuide.Application.DTOs.Settings
{
    public class UserSettingsDto
    {
        public bool PushNotificationsEnabled { get; set; }
        public bool DarkModeEnabled { get; set; }
        public string Language { get; set; } = "English";
        public string Currency { get; set; } = "USD";
    }
}
