using Microsoft.Extensions.DependencyInjection;
using TouristGuide.Application.Interfaces;
using TouristGuide.Application.Services;

namespace TouristGuide.Application
{
    public static class ApplicationServicesRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ITouristPlaceService,  TouristPlaceService>();
            services.AddScoped<IGuideService,         GuideService>();
            services.AddScoped<ITourService,          TourService>();
            services.AddScoped<ITourBookingService,   TourBookingService>();
            services.AddScoped<IAccommodationService, AccommodationService>();
            services.AddScoped<IStayBookingService,   StayBookingService>();
            services.AddScoped<ITripService,          TripService>();
            services.AddScoped<IFavoriteService,      FavoriteService>();
            services.AddScoped<IReviewService,        ReviewService>();
            services.AddScoped<INotificationService,  NotificationService>();
            services.AddScoped<IUserSettingsService,  UserSettingsService>();
            services.AddScoped<IAiService,            AiService>();

            return services;
        }
    }
}
