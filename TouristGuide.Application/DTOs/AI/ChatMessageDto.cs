namespace TouristGuide.Application.DTOs.AI
{
    public class ChatMessageDto
    {
        public string Role { get; set; } = string.Empty;   // "User" | "Assistant"
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }

    public class SendMessageDto
    {
        public string Content { get; set; } = string.Empty;
        public int? SessionId { get; set; }   // null = start a new session
    }

    public class ChatResponseDto
    {
        public int SessionId { get; set; }
        public string Reply { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
