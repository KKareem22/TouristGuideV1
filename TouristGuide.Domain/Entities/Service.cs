using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class Service : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ServiceType { get; set; } = string.Empty;

        public int ServiceProviderProfileId { get; set; }
        public ServiceProviderProfile ServiceProviderProfile { get; set; } = null!;

        public ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();
    }
}
