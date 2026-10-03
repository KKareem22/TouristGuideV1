using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GuidesController : BaseApiController
    {
        private readonly IGuideService _guideService;

        public GuidesController(IGuideService guideService)
        {
            _guideService = guideService;
        }

        // GET: api/Guides
        // Optional query: ?language=English&specialty=Food
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<GuideProfileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? language  = null,
            [FromQuery] string? specialty = null)
        {
            var guides = await _guideService.GetAllAsync(language, specialty);
            return Ok(guides);
        }

        // GET: api/Guides/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GuideProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var guide = await _guideService.GetByIdAsync(id);
            return guide is null ? NotFound($"Guide {id} not found.") : Ok(guide);
        }

        // GET: api/Guides/me
        // Returns the authenticated guide's own profile.
        [HttpGet("me")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(GuideProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile()
        {
            var guide = await _guideService.GetByUserIdAsync(GetCurrentUserId());
            return guide is null ? NotFound("Guide profile not found.") : Ok(guide);
        }

        // PUT: api/Guides/me
        // Updates the authenticated guide's profile (bio, location, languages, specialties, price).
        [HttpPut("me")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(GuideProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyProfile([FromBody] GuideProfileDto dto)
        {
            try
            {
                var updated = await _guideService.UpdateAsync(GetCurrentUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Guides/{id}/availability
        // Returns the weekly availability schedule for a guide (Mon-Sun toggles).
        [HttpGet("{id:int}/availability")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(Dictionary<string, bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailability(int id)
        {
            var schedule = await _guideService.GetWeeklyAvailabilityAsync(id);
            // Serialize DayOfWeek enum keys as their string names for readability
            var result = schedule.ToDictionary(kv => kv.Key.ToString(), kv => kv.Value);
            return Ok(result);
        }

        // PUT: api/Guides/me/availability
        // Sets the authenticated guide's weekly availability schedule.
        // Body: { "Monday": true, "Tuesday": false, ... }
        [HttpPut("me/availability")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SetMyAvailability(
            [FromBody] Dictionary<string, bool> schedule)
        {
            try
            {
                // Convert string day-names to DayOfWeek enum
                var parsed = schedule.ToDictionary(
                    kv => Enum.Parse<DayOfWeek>(kv.Key, ignoreCase: true),
                    kv => kv.Value);

                await _guideService.SetWeeklyAvailabilityAsync(GetCurrentUserId(), parsed);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
