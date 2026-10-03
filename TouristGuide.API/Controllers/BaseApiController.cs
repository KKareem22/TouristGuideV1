using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace TouristGuide.API.Controllers
{
    /// <summary>
    /// Shared base for all authenticated controllers.
    /// Provides a single GetCurrentUserId() helper so every controller
    /// can extract the caller's Identity user-id from the JWT without
    /// repeating the ClaimTypes.Sub lookup.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Returns the authenticated user's ID (the "uid" claim written by AuthService).
        /// Throws if the token is missing or malformed — guards against unauthenticated calls
        /// that somehow bypass the [Authorize] filter.
        /// </summary>
        protected string GetCurrentUserId()
        {
            var uid = User.FindFirstValue("uid")
                   ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(uid))
                throw new UnauthorizedAccessException("User identity could not be resolved.");

            return uid;
        }
    }
}
