using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TouristGuide.Application.Interfaces;
using TouristGuide.Application.Models;
using TouristGuide.Domain.Interfaces;
using TouristGuide.Infrastructure.AI;
using TouristGuide.Infrastructure.Data;
using TouristGuide.Infrastructure.Repositories;

namespace TouristGuide.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── Main application DbContext ────────────────────────────────────
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // ── Generic Repository & Unit of Work ─────────────────────────────
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Gemini AI ─────────────────────────────────────────────────────
            // Bind GeminiSettings from appsettings.json so IOptions<GeminiSettings>
            // can be injected anywhere in the Infrastructure layer.
            services.Configure<GeminiSettings>(
                configuration.GetSection("GeminiSettings"));

            // Named HttpClient for Gemini — base address is pre-set so
            // GeminiClient only needs to append the ?key= query string.
            services.AddHttpClient<IGeminiClient, GeminiClient>(client =>
            {
                var baseUrl = configuration["GeminiSettings:BaseUrl"]
                              ?? "https://generativelanguage.googleapis.com";
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
