using TouristGuide.Domain.Common;

namespace TouristGuide.Domain.Entities
{
    public class ServiceProviderProfile : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;

        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
