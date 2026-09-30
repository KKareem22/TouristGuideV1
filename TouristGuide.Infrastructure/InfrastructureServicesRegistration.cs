using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouristGuide.Infrastructure.Data;

namespace TouristGuide.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Add the AppDbContext for the main application data
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            return services;
        }
    }
}
