using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TouristGuide.Infrastructure.Identity;

namespace TouristGuide.Infrastructure
{
    public static class IdentityServiceRegistration
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. تسجيل الـ DbContext الخاص بالـ Identity
            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection")));

            // 2. إعداد الـ Identity (Users & Roles)
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // يمكنك هنا إضافة قواعد لكلمة المرور مثلاً
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders(); // مهم لإنشاء توكنز لتغيير الباسورد أو تأكيد الإيميل مستقبلاً

            return services;
        }
    }
}
