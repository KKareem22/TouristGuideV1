using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToursController : BaseApiController
    {
        private readonly ITourService        _tourService;
        private readonly ITourBookingService _bookingService;

        public ToursController(
            ITourService        tourService,
            ITourBookingService bookingService)
        {
            _tourService    = tourService;
            _bookingService = bookingService;
        }

        // ── Tour CRUD ────────────────────────────────────────────────────────

        // GET: api/Tours
        // Optional query: ?guideProfileId=3&status=Active
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<TourDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int?    guideProfileId = null,
            [FromQuery] string? status         = null)
        {
            var tours = await _tourService.GetAllAsync(guideProfileId, status);
            return Ok(tours);
        }

        // GET: api/Tours/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TourDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var tour = await _tourService.GetByIdAsync(id);
            return tour is null ? NotFound($"Tour {id} not found.") : Ok(tour);
        }

        // POST: api/Tours
        // Creates a new tour under the authenticated guide's profile.
        [HttpPost]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(TourDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TourDto dto)
        {
            try
            {
                var created = await _tourService.CreateAsync(GetCurrentUserId(), dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Tours/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(TourDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update(int id, [FromBody] TourDto dto)
        {
            try
            {
                var updated = await _tourService.UpdateAsync(id, GetCurrentUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Tours/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _tourService.DeleteAsync(id, GetCurrentUserId());
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ── Booking endpoints ────────────────────────────────────────────────

        // POST: api/Tours/book
        // Tourists book a tour experience.
        [HttpPost("book")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(TourBookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Book([FromBody] CreateTourBookingDto dto)
        {
            try
            {
                var booking = await _bookingService.CreateAsync(GetCurrentUserId(), dto);
                return StatusCode(StatusCodes.Status201Created, booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Tours/my-bookings
        // Tourists view their own tour bookings.
        [HttpGet("my-bookings")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(IReadOnlyList<TourBookingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyBookings()
        {
            var bookings = await _bookingService.GetForTouristAsync(GetCurrentUserId());
            return Ok(bookings);
        }

        // GET: api/Tours/requests
        // Guides view incoming booking requests for their tours.
        [HttpGet("requests")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(IReadOnlyList<TourBookingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await _bookingService.GetRequestsForGuideAsync(GetCurrentUserId());
            return Ok(requests);
        }

        // PUT: api/Tours/requests/{bookingId}/accept
        // Guide accepts a pending booking request.
        [HttpPut("requests/{bookingId:int}/accept")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(TourBookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptRequest(int bookingId)
        {
            try
            {
                var result = await _bookingService.AcceptAsync(bookingId, GetCurrentUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Tours/requests/{bookingId}/decline
        // Guide declines a pending booking request.
        [HttpPut("requests/{bookingId:int}/decline")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(TourBookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeclineRequest(int bookingId)
        {
            try
            {
                var result = await _bookingService.DeclineAsync(bookingId, GetCurrentUserId());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
