using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.AI;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    /// <summary>
    /// Handles all AI-powered features:
    ///   1. Smart trip planning  — the "AI Trip Planner" screen
    ///   2. Travel chat assistant — the AI chat session flow
    ///   3. User preference management — persisted interests/budget/style
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tourist")]
    public class AiController : BaseApiController
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        // ── Trip Planning ────────────────────────────────────────────────────

        // POST: api/Ai/generate-trip
        // Submits the AI trip planner form and returns a fully generated itinerary.
        // Body: { destination, travelDate, travelerCount, budgetTier, interests[] }
        // The itinerary is NOT persisted until the tourist taps "Save this trip".
        [HttpPost("generate-trip")]
        [ProducesResponseType(typeof(GeneratedItineraryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GenerateTrip([FromBody] AiTripRequestDto request)
        {
            try
            {
                var itinerary = await _aiService.GenerateTripAsync(GetCurrentUserId(), request);
                return Ok(itinerary);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Ai/save-trip
        // Persists an AI-generated itinerary as a real Trip in the database.
        // Returns the new Trip's integer ID.
        // Body: GeneratedItineraryDto (the object returned by generate-trip)
        [HttpPost("save-trip")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SaveTrip([FromBody] GeneratedItineraryDto itinerary)
        {
            try
            {
                var tripId = await _aiService.SaveGeneratedTripAsync(
                    GetCurrentUserId(), itinerary);
                return StatusCode(StatusCodes.Status201Created, tripId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ── Chat Assistant ───────────────────────────────────────────────────

        // POST: api/Ai/chat
        // Sends a message to the AI travel assistant.
        // Pass sessionId = null to start a new conversation; the response returns
        // the session ID to include in all subsequent messages.
        // Body: { content, sessionId? }
        [HttpPost("chat")]
        [ProducesResponseType(typeof(ChatResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Chat([FromBody] SendMessageDto message)
        {
            try
            {
                var response = await _aiService.SendMessageAsync(GetCurrentUserId(), message);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Ai/chat/{sessionId}/history
        // Retrieves the full message history of a chat session (User + Assistant turns).
        [HttpGet("chat/{sessionId:int}/history")]
        [ProducesResponseType(typeof(IReadOnlyList<ChatMessageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetChatHistory(int sessionId)
        {
            try
            {
                var history = await _aiService.GetChatHistoryAsync(
                    GetCurrentUserId(), sessionId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ── User Preferences ─────────────────────────────────────────────────

        // GET: api/Ai/preferences
        // Returns the tourist's stored AI preferences (categories, budget tier, travel style).
        // Used to pre-populate the AI planner form on repeat visits.
        [HttpGet("preferences")]
        [ProducesResponseType(typeof(UserPreferenceDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPreferences()
        {
            var prefs = await _aiService.GetPreferencesAsync(GetCurrentUserId());
            return Ok(prefs);
        }

        // PUT: api/Ai/preferences
        // Saves/updates the tourist's AI preferences.
        // Body: { preferredCategories, preferredBudgetTier, travelStyle, travelGroupType, maxDailyActivities }
        [HttpPut("preferences")]
        [ProducesResponseType(typeof(UserPreferenceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePreferences([FromBody] UserPreferenceDto dto)
        {
            try
            {
                var updated = await _aiService.UpdatePreferencesAsync(GetCurrentUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
