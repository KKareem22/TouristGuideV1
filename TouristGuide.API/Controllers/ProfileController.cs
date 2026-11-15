using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristGuide.Application.DTOs.Profile;
using TouristGuide.Application.DTOs.Settings;
using TouristGuide.Application.Interfaces;

namespace TouristGuide.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IProfileService _profileService;
        private readonly IUserSettingsService _settingsService;
        private readonly IWebHostEnvironment _env;

        public ProfileController(
            IProfileService profileService,
            IUserSettingsService settingsService,
            IWebHostEnvironment env)
        {
            _profileService = profileService;
            _settingsService = settingsService;
            _env = env;
        }

        // GET: api/Profile
        [HttpGet]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var profile = await _profileService.GetFullProfileAsync(GetCurrentUserId());
                return Ok(profile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/Profile/settings
        [HttpGet("settings")]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSettings()
        {
            var settings = await _settingsService.GetAsync(GetCurrentUserId());
            return Ok(settings);
        }

        // PUT: api/Profile/settings
        [HttpPut("settings")]
        [ProducesResponseType(typeof(UserSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSettings([FromBody] UserSettingsDto dto)
        {
            try
            {
                var updated = await _settingsService.UpdateAsync(GetCurrentUserId(), dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT: api/Profile
        // Accepts multipart/form-data (FirstName, LastName, PhoneNumber, Bio, Location, ProfileImage)
        [HttpPut]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request)
        {
            try
            {
                string? imageUrl = null;

                if (request.ProfileImage is not null && request.ProfileImage.Length > 0)
                {
                    imageUrl = await SaveProfileImageAsync(request.ProfileImage);
                }

                var dto = new UpdateProfileDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    Bio = request.Bio,
                    Location = request.Location,
                    ProfileImageUrl = imageUrl
                };

                var updatedProfile = await _profileService.UpdateProfileAsync(GetCurrentUserId(), dto);
                return Ok(updatedProfile);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> SaveProfileImageAsync(IFormFile file)
        {
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
                throw new InvalidOperationException("Image size cannot exceed 5 MB.");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only .jpg, .jpeg, .png, and .webp images are allowed.");

            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "profiles");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"{Request.Scheme}://{Request.Host}/uploads/profiles/{fileName}";
        }
    }

    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
