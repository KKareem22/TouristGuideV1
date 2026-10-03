using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Notification;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : BaseApiController
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        // GET: api/Notifications
        // Returns all notifications for the authenticated user, newest first.
        // Backs the Notifications screen with the "Mark all read" button.
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var notifications = await _notificationService.GetForUserAsync(GetCurrentUserId());
            return Ok(notifications);
        }

        // PUT: api/Notifications/mark-all-read
        // Marks every unread notification as read for the authenticated user.
        [HttpPut("mark-all-read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllRead()
        {
            await _notificationService.MarkAllReadAsync(GetCurrentUserId());
            return NoContent();
        }

        // PUT: api/Notifications/{id}/mark-read
        // Marks a single notification as read.
        [HttpPut("{id:int}/mark-read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MarkRead(int id)
        {
            try
            {
                await _notificationService.MarkReadAsync(id, GetCurrentUserId());
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
