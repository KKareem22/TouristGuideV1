using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Stay;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tourist")]
    public class StayBookingsController : BaseApiController
    {
        private readonly IStayBookingService _stayBookingService;

        public StayBookingsController(IStayBookingService stayBookingService)
        {
            _stayBookingService = stayBookingService;
        }

        // GET: api/StayBookings
        // Returns all stay bookings for the authenticated tourist.
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<StayBookingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyBookings()
        {
            var bookings = await _stayBookingService.GetForTouristAsync(GetCurrentUserId());
            return Ok(bookings);
        }

        // POST: api/StayBookings
        // Tourists book a room at an accommodation.
        [HttpPost]
        [ProducesResponseType(typeof(StayBookingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateStayBookingDto dto)
        {
            try
            {
                var booking = await _stayBookingService.CreateAsync(GetCurrentUserId(), dto);
                return StatusCode(StatusCodes.Status201Created, booking);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/StayBookings/{id}/cancel
        // Cancels an existing stay booking.
        [HttpPut("{id:int}/cancel")]
        [ProducesResponseType(typeof(StayBookingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var cancelled = await _stayBookingService.CancelAsync(id, GetCurrentUserId());
                return Ok(cancelled);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
