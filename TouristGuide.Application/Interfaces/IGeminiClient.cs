using TouristGuide.Application.DTOs.AI;

namespace TouristGuide.Application.Interfaces
{
    /// <summary>
    /// Contract for the Gemini generative-AI HTTP gateway.
    /// Implemented in the Infrastructure layer so that raw HTTP details
    /// never leak into Application business logic.
    /// </summary>
    public interface IGeminiClient
    {
        /// <summary>
        /// Sends a structured trip-planning prompt to Gemini and returns
        /// the parsed itinerary. Throws <see cref="HttpRequestException"/>
        /// on network failure or non-2xx responses.
        /// </summary>
        Task<GeneratedItineraryDto> GenerateTripAsync(AiTripRequestDto request, CancellationToken ct = default);

        /// <summary>
        /// Sends a conversational message (with optional prior-turn history)
        /// to Gemini and returns the model's plain-text reply.
        /// </summary>
        Task<string> SendChatMessageAsync(string userMessage, IReadOnlyList<ChatMessageDto> history, CancellationToken ct = default);
    }
}
