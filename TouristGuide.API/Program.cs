using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using TouristGuide.Application;
using TouristGuide.API.Middleware;
using TouristGuide.Infrastructure;
using TouristGuide.Infrastructure.Data;
using TouristGuide.Infrastructure.Identity;

namespace TouristGuide.API
{
    public class Program
    {
        private const string CorsPolicyName = "AllowAll";

        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Global exception handling (must be registered before building the app) ──
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails(); // required by AddExceptionHandler

            // ── CORS ──────────────────────────────────────────────────────────
            // "AllowAll" — permissive policy for Flutter mobile client.
            // Allows any origin, method, and header (including Authorization).
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            // ── Layer service registrations ───────────────────────────────────
            builder.Services.AddControllers();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddApplicationServices();

            // ── Swagger with JWT Bearer support ───────────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Tourist Guide API", Version = "v1" });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token. Example: Bearer {token}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // ─────────────────────────────────────────────────────────────────
            var app = builder.Build();

            // ── Step 1: Migrate Identity DB → then seed roles ─────────────────
            // هذا الـ scope يعمل المهمتين اللي بتتوقف الثانية على الأولى:
            //   (a) MigrateAsync على AppIdentityDbContext يضمن إن جداول الـ Identity
            //       (AspNetRoles, AspNetUsers, ...) اتكريتت على كلاود DB الفاضية.
            //   (b) RoleManager يكريت الـ Roles بعد كده بأمان لأن الجداول موجودة.
            using (var scope = app.Services.CreateScope())
            {
                // (a) تطبيق أي migrations معلقة على داتابيز الـ Identity
                var identityDb = scope.ServiceProvider
                    .GetRequiredService<AppIdentityDbContext>();
                await identityDb.Database.MigrateAsync();

                // (b) إنشاء الـ Roles لو مش موجودة
                var roleManager = scope.ServiceProvider
                    .GetRequiredService<RoleManager<IdentityRole>>();
                foreach (var role in new[] { "Tourist", "TourGuide" })
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ── Seed database with Egyptian tourist data ───────────────────────
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var seederLogger = scope.ServiceProvider
                    .GetRequiredService<ILogger<Program>>();
                await DatabaseSeeder.SeedAsync(dbContext, seederLogger);
            }

            // ── HTTP pipeline (order is mandatory) ────────────────────────────
            //
            //  1. UseExceptionHandler  — wraps everything below; must be outermost
            //  2. UseCors              — before auth so OPTIONS preflight is never rejected
            //  3. UseSwagger           — dev tooling
            //  4. UseHttpsRedirection  — redirect before any real work
            //  5. UseAuthentication    — validate JWT
            //  6. UseAuthorization     — apply [Authorize] policies
            //  7. MapControllers       — route to action methods

            app.UseExceptionHandler();   // uses GlobalExceptionHandler registered above

            app.UseCors(CorsPolicyName);

            
            
            app.UseSwagger();
            app.UseSwaggerUI();
            

            app.UseHttpsRedirection();

            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads", "profiles");
            Directory.CreateDirectory(uploadsPath);
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }
    }
}
