using TouristGuide.Application.DTOs.AI;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    /// <summary>
    /// Stub AI service. Wire up OpenAI / Azure AI SDK calls in the actual integration phase.
    /// All persistence logic (sessions, messages, preferences, trip saving) is fully implemented.
    /// </summary>
    public class AiService : IAiService
    {
        private readonly IUnitOfWork _uow;

        public AiService(IUnitOfWork uow) => _uow = uow;

        public async Task<GeneratedItineraryDto> GenerateTripAsync(string userId, AiTripRequestDto request)
        {
            // Persist the request log
            var reqRepo = _uow.GetRepository<AiTripRequest, int>();
            var reqEntity = new AiTripRequest
            {
                UserId = userId, Destination = request.Destination,
                TravelDate = request.TravelDate, TravelerCount = request.TravelerCount,
                BudgetTier = Enum.TryParse<BudgetTier>(request.BudgetTier, true, out var bt)
                    ? bt : BudgetTier.Comfort,
                Interests = string.Join("|", request.Interests),
                RequestedAt = DateTime.UtcNow
            };
            reqRepo.Add(reqEntity);
            await _uow.SaveChangesAsync();

            // ── Stub response (replace with actual AI call) ──────────────────
            return new GeneratedItineraryDto
            {
                Destination = request.Destination,
                StartDate = request.TravelDate,
                EndDate = request.TravelDate.AddDays(3),
                TravelerCount = request.TravelerCount,
                EstimatedBudgetPerPerson = "~$280/person",
                Days = new List<GeneratedDayDto>
                {
                    new() { DayNumber = 1, Date = request.TravelDate, DayTitle = "Arrive & unwind",
                        Items = new List<GeneratedItemDto>
                        {
                            new() { Time = "09:00", Title = "Arrive in " + request.Destination, Description = "Transfer to your hotel.", EstimatedCost = 65, IconType = "car" },
                            new() { Time = "12:30", Title = "Lunch by the sea", Description = "Fresh seafood at a local trattoria.", EstimatedCost = 35, IconType = "fork" }
                        }
                    }
                }
            };
        }

        public async Task<int> SaveGeneratedTripAsync(string userId, GeneratedItineraryDto itinerary)
        {
            // Resolve TouristProfile
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles = await profileRepo.GetAllAsync();
            var profile = profiles.FirstOrDefault(p => p.UserId == userId)
                ?? throw new Exception("Tourist profile not found.");

            var tripRepo = _uow.GetRepository<Trip, int>();
            var trip = new Trip
            {
                Name = $"{itinerary.Destination} Trip",
                StartDate = itinerary.StartDate, EndDate = itinerary.EndDate,
                TravelerCount = itinerary.TravelerCount,
                EstimatedBudgetPerPerson = itinerary.EstimatedBudgetPerPerson,
                CoverImageUrl = itinerary.CoverImageUrl,
                IsAiGenerated = true, Status = TripStatus.Planned,
                TouristProfileId = profile.Id
            };
            tripRepo.Add(trip);
            await _uow.SaveChangesAsync();
            return trip.Id;
        }

        public async Task<ChatResponseDto> SendMessageAsync(string userId, SendMessageDto message)
        {
            var sessionRepo = _uow.GetRepository<AiChatSession, int>();
            AiChatSession session;
            if (message.SessionId.HasValue)
            {
                session = await sessionRepo.GetByIdAsync(message.SessionId.Value)
                    ?? throw new Exception("Chat session not found.");
            }
            else
            {
                session = new AiChatSession { UserId = userId, IsActive = true };
                sessionRepo.Add(session);
                await _uow.SaveChangesAsync();
            }

            var msgRepo = _uow.GetRepository<AiChatMessage, int>();
            msgRepo.Add(new AiChatMessage
            {
                SessionId = session.Id, Role = AiMessageRole.User,
                Content = message.Content, Timestamp = DateTime.UtcNow
            });

            // ── Stub AI reply (replace with actual AI call) ───────────────
            var reply = $"I'd love to help you plan your trip! Tell me more about {message.Content}.";
            msgRepo.Add(new AiChatMessage
            {
                SessionId = session.Id, Role = AiMessageRole.Assistant,
                Content = reply, Timestamp = DateTime.UtcNow
            });
            await _uow.SaveChangesAsync();

            return new ChatResponseDto { SessionId = session.Id, Reply = reply, Timestamp = DateTime.UtcNow };
        }

        public async Task<IReadOnlyList<ChatMessageDto>> GetChatHistoryAsync(string userId, int sessionId)
        {
            var msgRepo = _uow.GetRepository<AiChatMessage, int>();
            var all = await msgRepo.GetAllAsync();
            return all.Where(m => m.SessionId == sessionId)
                      .OrderBy(m => m.Timestamp)
                      .Select(m => new ChatMessageDto
                      {
                          Role = m.Role.ToString(), Content = m.Content, Timestamp = m.Timestamp
                      }).ToList();
        }

        public async Task<UserPreferenceDto> GetPreferencesAsync(string userId)
        {
            var repo = _uow.GetRepository<UserPreference, int>();
            var all = await repo.GetAllAsync();
            var pref = all.FirstOrDefault(p => p.UserId == userId);
            if (pref is null) return new UserPreferenceDto();
            return MapPrefToDto(pref);
        }

        public async Task<UserPreferenceDto> UpdatePreferencesAsync(string userId, UserPreferenceDto dto)
        {
            var repo = _uow.GetRepository<UserPreference, int>();
            var all = await repo.GetAllAsync();
            var pref = all.FirstOrDefault(p => p.UserId == userId);
            if (pref is null)
            {
                pref = new UserPreference { UserId = userId };
                repo.Add(pref);
            }
            pref.PreferredCategories = dto.PreferredCategories;
            pref.PreferredBudgetTier = dto.PreferredBudgetTier;
            pref.TravelStyle = dto.TravelStyle;
            pref.MaxDailyActivities = dto.MaxDailyActivities;
            if (Enum.TryParse<TravelGroupType>(dto.TravelGroupType, true, out var tgt))
                pref.TravelGroupType = tgt;
            pref.UpdatedAt = DateTime.UtcNow;
            repo.Update(pref);
            await _uow.SaveChangesAsync();
            return MapPrefToDto(pref);
        }

        private static UserPreferenceDto MapPrefToDto(UserPreference p) => new()
        {
            PreferredCategories = p.PreferredCategories,
            PreferredBudgetTier = p.PreferredBudgetTier,
            TravelStyle = p.TravelStyle,
            TravelGroupType = p.TravelGroupType.ToString(),
            MaxDailyActivities = p.MaxDailyActivities
        };
    }
}
