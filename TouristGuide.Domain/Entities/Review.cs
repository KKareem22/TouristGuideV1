using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Review : BaseEntity<int>
    {
        public ReferenceType ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public int TouristProfileId { get; set; }
        public TouristProfile TouristProfile { get; set; } = null!;
    }
}
