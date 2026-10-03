using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Favorite;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Tourist")]
    public class FavoritesController : BaseApiController
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        // GET: api/Favorites
        // Returns all saved favorites for the authenticated tourist.
        // Optional query: ?entityType=Place  (Place | Guide | Accommodation)
        // Backs the three-tab Favorites screen.
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<UserFavoriteDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] string? entityType = null)
        {
            var favorites = await _favoriteService.GetAllAsync(GetCurrentUserId(), entityType);
            return Ok(favorites);
        }

        // POST: api/Favorites/toggle
        // Adds the item to favorites if it isn't saved; removes it if it is.
        // Body: { "entityType": "Place", "entityId": 5 }
        [HttpPost("toggle")]
        [ProducesResponseType(typeof(UserFavoriteDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Toggle([FromBody] ToggleFavoriteDto dto)
        {
            try
            {
                var result = await _favoriteService.ToggleAsync(GetCurrentUserId(), dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Favorites/check?entityType=Guide&entityId=3
        // Returns true/false — used to decide whether to show a filled or outlined heart icon.
        [HttpGet("check")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> Check(
            [FromQuery] string entityType,
            [FromQuery] int    entityId)
        {
            var isFavorite = await _favoriteService.IsFavoriteAsync(
                GetCurrentUserId(), entityType, entityId);
            return Ok(isFavorite);
        }
    }
}
