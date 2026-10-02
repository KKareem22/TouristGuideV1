using Microsoft.EntityFrameworkCore;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        // ── Core Profiles ────────────────────────────────────────────────
        public DbSet<TouristProfile> TouristProfiles { get; set; }
        public DbSet<GuideProfile> GuideProfiles { get; set; }
        public DbSet<ServiceProviderProfile> ServiceProviderProfiles { get; set; }

        // ── Places & Activities ───────────────────────────────────────────
        public DbSet<TouristPlace> TouristPlaces { get; set; }
        public DbSet<Activity> Activities { get; set; }

        // ── Trips & Itinerary ─────────────────────────────────────────────
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripDay> TripDays { get; set; }
        public DbSet<ItineraryItem> ItineraryItems { get; set; }

        // ── Tours (Guide-led Experiences) ─────────────────────────────────
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourItineraryStep> TourItinerarySteps { get; set; }
        public DbSet<TourBooking> TourBookings { get; set; }
        public DbSet<GuideWeeklyAvailability> GuideWeeklyAvailabilities { get; set; }

        // ── Stays / Accommodation ─────────────────────────────────────────
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<AccommodationAmenity> AccommodationAmenities { get; set; }
        public DbSet<AccommodationImage> AccommodationImages { get; set; }
        public DbSet<StayBooking> StayBookings { get; set; }

        // ── Services (non-guide) ──────────────────────────────────────────
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceBooking> ServiceBookings { get; set; }

        // ── Social / User ─────────────────────────────────────────────────
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserFavorite> UserFavorites { get; set; }
        public DbSet<UserSettings> UserSettings { get; set; }

        // ── AI ────────────────────────────────────────────────────────────
        public DbSet<UserPreference> UserPreferences { get; set; }
        public DbSet<AiTripRequest> AiTripRequests { get; set; }
        public DbSet<AiChatSession> AiChatSessions { get; set; }
        public DbSet<AiChatMessage> AiChatMessages { get; set; }
        public DbSet<AiRecommendationLog> AiRecommendationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
