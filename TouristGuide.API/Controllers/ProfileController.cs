using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Settings;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    /// <summary>
    /// Handles the authenticated user's personal Settings screen.
    /// Applies to both Tourist and TourGuide roles.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IUserSettingsService _settingsService;

        public ProfileController(IUserSettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        // GET: api/Profile/settings
        // Returns the authenticated user's app settings.
        // Auto-creates default settings (EN, USD, push on, dark mode off) on first call.
        [HttpGet("settings")]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _settingsService.GetAsync(GetCurrentUserId());
            return Ok(settings);
        }

        // PUT: api/Profile/settings
        // Updates the authenticated user's app settings.
        // Body: { pushNotificationsEnabled, darkModeEnabled, language, currency }
        [HttpPut("settings")]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsDto dto)
        {
            try
            {
                var updated = await _settingsService.UpdateAsync(GetCurrentUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
