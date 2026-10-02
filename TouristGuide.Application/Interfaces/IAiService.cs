using TouristGuide.Application.DTOs.AI;

namespace TouristGuide.Application.Interfaces
{
    public interface IAiService
    {
        Task<GeneratedItineraryDto> GenerateTripAsync(string userId, AiTripRequestDto request);
        Task<int> SaveGeneratedTripAsync(string userId, GeneratedItineraryDto itinerary);
        Task<ChatResponseDto> SendMessageAsync(string userId, SendMessageDto message);
        Task<IReadOnlyList<ChatMessageDto>> GetChatHistoryAsync(string userId, int sessionId);
        Task<UserPreferenceDto> GetPreferencesAsync(string userId);
        Task<UserPreferenceDto> UpdatePreferencesAsync(string userId, UserPreferenceDto dto);
    }
}
