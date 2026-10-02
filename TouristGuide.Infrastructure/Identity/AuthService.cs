using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TouristGuide.Application.DTOs.Auth;
using TouristGuide.Application.Interfaces;
using TouristGuide.Application.Models;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _uow;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _uow = uow;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate role
            var role = request.Role is "Tourist" or "TourGuide"
                ? request.Role
                : throw new Exception("Role must be 'Tourist' or 'TourGuide'.");

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                throw new Exception($"Email '{request.Email}' is already registered.");

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName  = request.LastName,
                Email     = request.Email,
                UserName  = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
            }

            // Assign identity role (roles are seeded at startup)
            await _userManager.AddToRoleAsync(user, role);

            // Create the matching domain profile
            if (role == "Tourist")
            {
                var profileRepo = _uow.GetRepository<TouristProfile, int>();
                profileRepo.Add(new TouristProfile { UserId = user.Id });
                await _uow.SaveChangesAsync();
            }
            else // TourGuide
            {
                var guideRepo = _uow.GetRepository<GuideProfile, int>();
                guideRepo.Add(new GuideProfile
                {
                    UserId = user.Id,
                    Bio = string.Empty,
                    Location = string.Empty,
                    Languages = string.Empty,
                    Specialties = string.Empty
                });
                await _uow.SaveChangesAsync();
            }

            return await BuildAuthResponseAsync(user);
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new Exception("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
                throw new Exception("Invalid email or password.");

            return await BuildAuthResponseAsync(user);
        }

        // ─────────────────────────────────────────────────────────────
        private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
        {
            var roles  = await _userManager.GetRolesAsync(user);
            var token  = GenerateJwtToken(user, roles);
            var expiry = DateTime.UtcNow.AddDays(_jwtSettings.DurationInDays);

            return new AuthResponse
            {
                UserId    = user.Id,
                Email     = user.Email!,
                FirstName = user.FirstName,
                LastName  = user.LastName,
                Token     = token,
                ExpiresAt = expiry
            };
        }

        private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name,               $"{user.FirstName} {user.LastName}"),
                new Claim("uid",                         user.Id)
            };
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(_jwtSettings.DurationInDays);

            var token = new JwtSecurityToken(
                issuer:             _jwtSettings.Issuer,
                audience:           _jwtSettings.Audience,
                claims:             claims,
                expires:            expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
