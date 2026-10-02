using TouristGuide.Domain.Common;
using TouristGuide.Domain.Enums;

namespace TouristGuide.Domain.Entities
{
    public class Trip : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TripStatus Status { get; set; }
        public int TravelerCount { get; set; } = 1;
        public string? CoverImageUrl { get; set; }
        public string? EstimatedBudgetPerPerson { get; set; }   // "~$280/person"
        public bool IsAiGenerated { get; set; } = false;

        public int TouristProfileId { get; set; }
        public TouristProfile TouristProfile { get; set; } = null!;

        // Navigation
        public ICollection<TripDay> TripDays { get; set; } = new List<TripDay>();
    }
}
