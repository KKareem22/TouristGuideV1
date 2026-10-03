using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Stay;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationsController : BaseApiController
    {
        private readonly IAccommodationService _accommodationService;

        public AccommodationsController(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
        }

        // GET: api/Accommodations
        // Mirrors the "Find a Stay" screen: ?destination=Santorini&checkIn=2026-06-14&checkOut=2026-06-21&guests=2&type=Resort
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<AccommodationDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] string?   destination = null,
            [FromQuery] DateTime? checkIn     = null,
            [FromQuery] DateTime? checkOut    = null,
            [FromQuery] int?      guests      = null,
            [FromQuery] string?   type        = null)
        {
            var results = await _accommodationService.SearchAsync(
                destination, checkIn, checkOut, guests, type);
            return Ok(results);
        }

        // GET: api/Accommodations/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AccommodationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var accommodation = await _accommodationService.GetByIdAsync(id);
            return accommodation is null
                ? NotFound($"Accommodation {id} not found.")
                : Ok(accommodation);
        }

        // GET: api/Accommodations/{id}/rooms
        // Returns all rooms for a given accommodation.
        [HttpGet("{id:int}/rooms")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<RoomDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRooms(int id)
        {
            var rooms = await _accommodationService.GetRoomsAsync(id);
            return Ok(rooms);
        }

        // POST: api/Accommodations
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(AccommodationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AccommodationDto dto)
        {
            try
            {
                var created = await _accommodationService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Accommodations/{id}
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(AccommodationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] AccommodationDto dto)
        {
            try
            {
                var updated = await _accommodationService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
