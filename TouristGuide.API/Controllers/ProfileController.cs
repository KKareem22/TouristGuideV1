using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Profile;
using TouristGuide.Application.DTOs.Settings;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService      _profileService;
        private readonly IUserSettingsService _settingsService;

        public ProfileController(
            IProfileService      profileService,
            IUserSettingsService settingsService)
        {
            _profileService  = profileService;
            _settingsService = settingsService;
        }

        // GET: api/Profile
        // Returns the authenticated user's comprehensive profile:
        // Identity data, role, domain profile (Tourist or Guide), AI preferences, and app settings.
        [HttpGet]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _profileService.GetFullProfileAsync(GetCurrentUserId());
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Profile/settings
        // Returns only the authenticated user's app settings (lightweight).
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

        // PUT: api/Profile
        // Updates the authenticated user's profile (FirstName, LastName, PhoneNumber, Bio, Location).
        [HttpPut]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                var updatedProfile = await _profileService.UpdateProfileAsync(GetCurrentUserId(), dto);
                return Ok(updatedProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
