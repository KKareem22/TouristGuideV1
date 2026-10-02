using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TouristGuide.Application.DTOs.Auth;
using TouristGuide.Application.Interfaces;
using TouristGuide.Application.Models;

namespace TouristGuide.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Check if email is already taken
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser is not null)
                throw new Exception($"Email '{request.Email}' is already registered.");

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new Exception($"Registration failed: {errors}");
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
        // Private helpers
        // ─────────────────────────────────────────────────────────────

        private async Task<AuthResponse> BuildAuthResponseAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, roles);
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

            // Add each role as a separate claim
            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var key     = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds   = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
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
