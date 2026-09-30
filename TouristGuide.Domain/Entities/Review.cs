using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Review:BaseEntity
    {
        public ReferenceType ReferenceType { get; set; }
        public int ReferenceId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;

        public int TouristId { get; set; }
        public Tourist Tourist { get; set; } = null!;
    }
}