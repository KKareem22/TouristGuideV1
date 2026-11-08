using TouristGuide.Application.DTOs.AI;
using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.DTOs.Settings;

namespace TouristGuide.Application.DTOs.Profile
{
    /// <summary>
    /// Comprehensive profile response returned by GET /api/Profile.
    /// Combines Identity data, role, domain-specific profile, preferences, and app settings.
    /// </summary>
    public class UserProfileDto
    {
        // ── Identity data ────────────────────────────────────────────────────
        public string UserId    { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName  { get; set; } = string.Empty;
        public string FullName  { get; set; } = string.Empty;
        public string Email     { get; set; } = string.Empty;

        /// <summary>Phone number from ASP.NET Core Identity.</summary>
        public string? PhoneNumber { get; set; }

        /// <summary>Profile picture URL — sourced from the domain profile record.</summary>
        public string? ProfileImageUrl { get; set; }

        // ── Role ─────────────────────────────────────────────────────────────
        /// <summary>"Tourist" or "TourGuide".</summary>
        public string Role { get; set; } = string.Empty;

        /// <summary>True if the email address has been confirmed in Identity.</summary>
        public bool IsEmailConfirmed { get; set; }

        // ── Tourist-specific data (null when Role = "TourGuide") ─────────────
        public TouristProfileSummaryDto? TouristProfile { get; set; }

        /// <summary>Persisted AI travel preferences — only for tourists.</summary>
        public UserPreferenceDto? Preferences { get; set; }

        // ── Guide-specific data (null when Role = "Tourist") ─────────────────
        public GuideProfileDto? GuideProfile { get; set; }

        // ── App settings (applies to both roles) ─────────────────────────────
        public UserSettingsDto Settings { get; set; } = new();
    }

    /// <summary>
    /// Flat summary of a tourist's domain profile.
    /// Avoids exposing heavy navigation collections across the API boundary.
    /// </summary>
    public class TouristProfileSummaryDto
    {
        public int     Id               { get; set; }
        public string? DisplayLocation  { get; set; }
        public string? Bio              { get; set; }
        public string? Nationality      { get; set; }
        public string? PreferredLanguage { get; set; }
        public DateTime? DateOfBirth    { get; set; }
        public int     TotalTrips       { get; set; }
        public int     TotalFavorites   { get; set; }
        public int     TotalReviews     { get; set; }
    }
}
