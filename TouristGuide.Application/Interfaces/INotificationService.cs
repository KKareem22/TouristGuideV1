using TouristGuide.Application.DTOs.Notification;

namespace TouristGuide.Application.Interfaces
{
    public interface INotificationService
    {
        Task<IReadOnlyList<NotificationDto>> GetForUserAsync(string userId);
        Task MarkAllReadAsync(string userId);
        Task MarkReadAsync(int id, string userId);
        Task<NotificationDto> CreateAsync(string userId, string title, string message, string notificationType);
    }
}
