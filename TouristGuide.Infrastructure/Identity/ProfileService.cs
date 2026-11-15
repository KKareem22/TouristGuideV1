using Microsoft.AspNetCore.Identity;
using TouristGuide.Application.DTOs.AI;
using TouristGuide.Application.DTOs.Guide;
using TouristGuide.Application.DTOs.Profile;
using TouristGuide.Application.DTOs.Settings;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Infrastructure.Identity
{
    /// <summary>
    /// Assembles a <see cref="UserProfileDto"/> by combining:
    ///   - ASP.NET Core Identity data  (UserManager)
    ///   - Role information            (UserManager.GetRolesAsync)
    ///   - Domain profile data         (TouristProfile / GuideProfile via IUnitOfWork)
    ///   - AI preferences              (UserPreference via IUnitOfWork)
    ///   - App settings                (UserSettings via IUnitOfWork)
    ///
    /// Lives in Infrastructure because it has a hard dependency on
    /// <c>UserManager&lt;ApplicationUser&gt;</c>, which is an Identity type.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _uow;

        public ProfileService(UserManager<ApplicationUser> userManager, IUnitOfWork uow)
        {
            _userManager = userManager;
            _uow = uow;
        }

        public async Task<UserProfileDto> GetFullProfileAsync(string userId)
        {
            // ── 1. Identity user ─────────────────────────────────────────────
            var user = await _userManager.FindByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User '{userId}' not found.");

            // ── 2. Role ──────────────────────────────────────────────────────
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty; // Tourist | TourGuide

            // ── 3. App settings (auto-created if missing) ────────────────────
            var settingsRepo = _uow.GetRepository<UserSettings, int>();
            var allSettings = await settingsRepo.GetAllAsync();
            var settings = allSettings.FirstOrDefault(s => s.UserId == userId);

            if (settings is null)
            {
                settings = new UserSettings { UserId = userId };
                settingsRepo.Add(settings);
                await _uow.SaveChangesAsync();
            }

            var settingsDto = new UserSettingsDto
            {
                PushNotificationsEnabled = settings.PushNotificationsEnabled,
                DarkModeEnabled = settings.DarkModeEnabled,
                Language = settings.Language,
                Currency = settings.Currency
            };

            // ── 4. Build base DTO ────────────────────────────────────────────
            var dto = new UserProfileDto
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                IsEmailConfirmed = user.EmailConfirmed,
                Role = role,
                Settings = settingsDto
            };

            // ── 5. Role-specific enrichment ──────────────────────────────────
            if (role == "Tourist")
            {
                await EnrichTouristAsync(dto, userId);
            }
            else if (role == "TourGuide")
            {
                await EnrichGuideAsync(dto, userId);
            }

            return dto;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Tourist enrichment
        // ─────────────────────────────────────────────────────────────────────

        private async Task EnrichTouristAsync(UserProfileDto dto, string userId)
        {
            // Domain profile
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var allProfiles = await profileRepo.GetAllAsync();
            var profile = allProfiles.FirstOrDefault(p => p.UserId == userId);

            if (profile is not null)
            {
                dto.ProfileImageUrl = profile.ProfileImageUrl;

                dto.TouristProfile = new TouristProfileSummaryDto
                {
                    Id = profile.Id,
                    DisplayLocation = profile.DisplayLocation,
                    Bio = profile.Bio,
                    Nationality = profile.Nationality,
                    PreferredLanguage = profile.PreferredLanguage,
                    DateOfBirth = profile.DateOfBirth,
                    TotalTrips = profile.Trips.Count,
                    TotalFavorites = profile.Favorites.Count,
                    TotalReviews = profile.Reviews.Count
                };
            }

            // AI preferences
            var prefRepo = _uow.GetRepository<UserPreference, int>();
            var allPrefs = await prefRepo.GetAllAsync();
            var pref = allPrefs.FirstOrDefault(p => p.UserId == userId);

            dto.Preferences = pref is null
                ? new UserPreferenceDto()
                : new UserPreferenceDto
                {
                    PreferredCategories = pref.PreferredCategories,
                    PreferredBudgetTier = pref.PreferredBudgetTier,
                    TravelStyle = pref.TravelStyle,
                    TravelGroupType = pref.TravelGroupType.ToString(),
                    MaxDailyActivities = pref.MaxDailyActivities
                };
        }

        // ─────────────────────────────────────────────────────────────────────
        // Guide enrichment
        // ─────────────────────────────────────────────────────────────────────

        private async Task EnrichGuideAsync(UserProfileDto dto, string userId)
        {
            var guideRepo = _uow.GetRepository<GuideProfile, int>();
            var allGuides = await guideRepo.GetAllAsync();
            var guide = allGuides.FirstOrDefault(g => g.UserId == userId && !g.IsDeleted);

            if (guide is null) return;

            dto.ProfileImageUrl = guide.ProfileImageUrl;

            dto.GuideProfile = new GuideProfileDto
            {
                Id = guide.Id,
                UserId = guide.UserId,
                Bio = guide.Bio,
                Location = guide.Location,
                Languages = guide.Languages,
                Specialties = guide.Specialties,
                IsVerified = guide.IsVerified,
                ExperienceYears = guide.ExperienceYears,
                HappyTravelers = guide.HappyTravelers,
                AverageRating = guide.AverageRating,
                ReviewCount = guide.ReviewCount,
                PricePerPerson = guide.PricePerPerson,
                ProfileImageUrl = guide.ProfileImageUrl,
                CoverImageUrl = guide.CoverImageUrl
            };
        }



        public async Task<UserProfileDto> UpdateProfileAsync(string userId, UpdateProfileDto dto)
        {
            // ── 1. تحديث الاسم ورقم الهاتف في جدول الـ Identity ───────────────
            var user = await _userManager.FindByIdAsync(userId)
                       ?? throw new KeyNotFoundException($"User '{userId}' not found.");

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName.Trim();

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.PhoneNumber = dto.PhoneNumber.Trim();

            var identityResult = await _userManager.UpdateAsync(user);
            if (!identityResult.Succeeded)
            {
                var errors = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to update user: {errors}");
            }

            // ── 2. تحديث الـ Bio والـ Location حسب نوع اليوزر (Tourist أو TourGuide)
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? string.Empty;

            if (role == "Tourist")
            {
                var profileRepo = _uow.GetRepository<TouristProfile, int>();
                var allProfiles = await profileRepo.GetAllAsync();
                var profile = allProfiles.FirstOrDefault(p => p.UserId == userId);

                if (profile is not null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.Bio))
                        profile.Bio = dto.Bio.Trim();

                    if (!string.IsNullOrWhiteSpace(dto.Location))
                        profile.DisplayLocation = dto.Location.Trim();

                    if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl))
                        profile.ProfileImageUrl = dto.ProfileImageUrl.Trim();

                    await _uow.SaveChangesAsync();
                }
            }
            else if (role == "TourGuide")
            {
                var guideRepo = _uow.GetRepository<GuideProfile, int>();
                var allGuides = await guideRepo.GetAllAsync();
                var guide = allGuides.FirstOrDefault(g => g.UserId == userId && !g.IsDeleted);

                if (guide is not null)
                {
                    if (!string.IsNullOrWhiteSpace(dto.Bio))
                        guide.Bio = dto.Bio.Trim();

                    if (!string.IsNullOrWhiteSpace(dto.Location))
                        guide.Location = dto.Location.Trim();

                    if (!string.IsNullOrWhiteSpace(dto.ProfileImageUrl))
                        guide.ProfileImageUrl = dto.ProfileImageUrl.Trim();

                    await _uow.SaveChangesAsync();
                }
            }

            // ── 3. نرجع البروفايل كامل بعد التحديث عشان الفرونت يحدث الشاشة فوراً
            return await GetFullProfileAsync(userId);
        }
    }
}
