using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Place;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : BaseApiController
    {
        private readonly ITouristPlaceService _placeService;

        public PlacesController(ITouristPlaceService placeService)
        {
            _placeService = placeService;
        }

        // GET: api/Places
        // Optional query: ?category=Beaches&featured=true
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<TouristPlaceDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? category = null,
            [FromQuery] bool?   featured = null)
        {
            var places = await _placeService.GetAllAsync(category, featured);
            return Ok(places);
        }

        // GET: api/Places/{id}
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TouristPlaceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var place = await _placeService.GetByIdAsync(id);
            return place is null ? NotFound($"Place {id} not found.") : Ok(place);
        }

        // GET: api/Places/{id}/activities
        [HttpGet("{id:int}/activities")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<ActivityDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActivities(int id)
        {
            var activities = await _placeService.GetActivitiesAsync(id);
            return Ok(activities);
        }

        // POST: api/Places
        [HttpPost]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(TouristPlaceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] TouristPlaceDto dto)
        {
            try
            {
                var created = await _placeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Places/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(typeof(TouristPlaceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] TouristPlaceDto dto)
        {
            try
            {
                var updated = await _placeService.UpdateAsync(id, dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Places/{id}
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TourGuide")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _placeService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
