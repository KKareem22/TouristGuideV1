using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Trip;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tourist")]
    public class TripsController : BaseApiController
    {
        private readonly ITripService _tripService;

        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        // GET: api/Trips
        // Returns the authenticated tourist's trips.
        // Query: ?upcoming=true  (true = upcoming/active, false = past)
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<TripDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTrips([FromQuery] bool upcoming = true)
        {
            var trips = await _tripService.GetForTouristAsync(GetCurrentUserId(), upcoming);
            return Ok(trips);
        }

        // GET: api/Trips/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _tripService.GetByIdAsync(id);
            return trip is null ? NotFound($"Trip {id} not found.") : Ok(trip);
        }

        // POST: api/Trips
        // Manually creates a new trip (not AI-generated).
        [HttpPost]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TripDto dto)
        {
            try
            {
                var created = await _tripService.CreateAsync(GetCurrentUserId(), dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Trips/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(TripDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] TripDto dto)
        {
            try
            {
                var updated = await _tripService.UpdateAsync(id, GetCurrentUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Trips/{id}
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tripService.DeleteAsync(id, GetCurrentUserId());
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
