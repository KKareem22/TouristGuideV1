using TouristGuide.Application.DTOs.Notification;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _uow;

        public NotificationService(IUnitOfWork uow) => _uow = uow;

        public async Task<IReadOnlyList<NotificationDto>> GetForUserAsync(string userId)
        {
            var repo = _uow.GetRepository<Notification, int>();
            var all = await repo.GetAllAsync();
            return all.Where(n => n.UserId == userId && !n.IsDeleted)
                      .OrderByDescending(n => n.CreatedAt)
                      .Select(MapToDto).ToList();
        }

        public async Task MarkAllReadAsync(string userId)
        {
            var repo = _uow.GetRepository<Notification, int>();
            var all = await repo.GetAllAsync();
            var unread = all.Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted).ToList();
            foreach (var n in unread) { n.IsRead = true; n.UpdatedAt = DateTime.UtcNow; repo.Update(n); }
            await _uow.SaveChangesAsync();
        }

        public async Task MarkReadAsync(int id, string userId)
        {
            var repo = _uow.GetRepository<Notification, int>();
            var n = await repo.GetByIdAsync(id) ?? throw new Exception($"Notification {id} not found.");
            if (n.UserId != userId) throw new UnauthorizedAccessException();
            n.IsRead = true; n.UpdatedAt = DateTime.UtcNow;
            repo.Update(n);
            await _uow.SaveChangesAsync();
        }

        public async Task<NotificationDto> CreateAsync(string userId, string title, string message, string notificationType)
        {
            var repo = _uow.GetRepository<Notification, int>();
            var entity = new Notification
            {
                UserId = userId, Title = title, Message = message,
                NotificationType = Enum.TryParse<NotificationType>(notificationType, true, out var nt)
                    ? nt : NotificationType.System
            };
            repo.Add(entity);
            await _uow.SaveChangesAsync();
            return MapToDto(entity);
        }

        private static NotificationDto MapToDto(Notification n) => new()
        {
            Id = n.Id, Title = n.Title, Message = n.Message,
            NotificationType = n.NotificationType.ToString(),
            IsRead = n.IsRead, CreatedAt = n.CreatedAt
        };
    }
}
