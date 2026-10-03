using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Review;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : BaseApiController
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // GET: api/Reviews?referenceType=Guide&referenceId=1
        // Returns all reviews for a given entity (Place / Guide / Accommodation / Tour).
        // Backs the "Reviews" screen shown after a tour or guide card.
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IReadOnlyList<ReviewDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetForEntity(
            [FromQuery] string referenceType,
            [FromQuery] int    referenceId)
        {
            try
            {
                var reviews = await _reviewService.GetForEntityAsync(referenceType, referenceId);
                return Ok(reviews);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Reviews
        // Authenticated tourists submit a review.
        // Body: { "referenceType": "Guide", "referenceId": 1, "rating": 5, "comment": "..." }
        [HttpPost]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
        {
            try
            {
                var review = await _reviewService.CreateAsync(GetCurrentUserId(), dto);
                return StatusCode(StatusCodes.Status201Created, review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE: api/Reviews/{id}
        // Tourists can delete their own reviews.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _reviewService.DeleteAsync(id, GetCurrentUserId());
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("You can only delete your own reviews.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
