using TouristGuide.Application.DTOs.Profile;

namespace TouristGuide.Application.Interfaces
{
    /// <summary>
    /// Assembles a comprehensive user profile by combining Identity data,
    /// role information, domain-specific profile details, AI preferences,
    /// and app settings into a single <see cref="UserProfileDto"/>.
    ///
    /// Implemented in the Infrastructure layer because it requires direct
    /// access to <c>UserManager&lt;ApplicationUser&gt;</c> and
    /// <c>IUnitOfWork</c> (two Infrastructure dependencies).
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Returns the complete profile for the given Identity user ID.
        /// Throws <see cref="KeyNotFoundException"/> when the user does not exist.
        /// </summary>
        Task<UserProfileDto> GetFullProfileAsync(string userId);
        Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    }
}
