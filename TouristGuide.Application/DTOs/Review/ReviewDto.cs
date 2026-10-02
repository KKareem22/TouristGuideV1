namespace TouristGuide.Application.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string ReferenceType { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
    }

    public class CreateReviewDto
    {
        public string ReferenceType { get; set; } = string.Empty;
        public int ReferenceId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
