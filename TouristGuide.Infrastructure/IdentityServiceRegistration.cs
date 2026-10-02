using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TouristGuide.Application.Interfaces;
using TouristGuide.Application.Models;
using TouristGuide.Infrastructure.Identity;

namespace TouristGuide.Infrastructure
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Bind JwtSettings from appsettings.json
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // 2. Register the Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            // 3. Configure ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit           = true;
                options.Password.RequiredLength         = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase       = true;
                options.Password.RequireLowercase       = true;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();

            // 4. Configure JWT Bearer authentication
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey         = new SymmetricSecurityKey(key),
                    ValidateIssuer           = true,
                    ValidIssuer              = jwtSettings.Issuer,
                    ValidateAudience         = true,
                    ValidAudience            = jwtSettings.Audience,
                    ValidateLifetime         = true,
                    ClockSkew                = TimeSpan.Zero   // no tolerance — token expires exactly on time
                };
            });

            // 5. Register AuthService
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
