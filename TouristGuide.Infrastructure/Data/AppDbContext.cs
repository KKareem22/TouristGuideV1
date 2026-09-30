using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TouristGuide.Domain.Entities;

namespace TouristGuide.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Tourist> Tourists { get; set; }
        public DbSet<GuideProfile> GuideProfiles { get; set; }
        public DbSet<ServiceProviderProfile> ServiceProviderProfiles { get; set; }
        public DbSet<TouristPlace> TouristPlaces { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripDay> TripDays { get; set; }
        public DbSet<ItineraryItem> ItineraryItems { get; set; }
        public DbSet<GuideAvailability> GuideAvailabilities { get; set; }
        public DbSet<GuideBooking> GuideBookings { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceBooking> ServiceBookings { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
