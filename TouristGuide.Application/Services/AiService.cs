using TouristGuide.Application.DTOs.AI;
using TouristGuide.Application.Interfaces;
using TouristGuide.Domain.Entities;
using TouristGuide.Domain.Enums;
using TouristGuide.Domain.Interfaces;

namespace TouristGuide.Application.Services
{
    /// <summary>
    /// Orchestrates AI features: trip planning, chat assistant, and user preferences.
    /// All HTTP communication is delegated to <see cref="IGeminiClient"/> (Infrastructure).
    /// All database persistence is handled through <see cref="IUnitOfWork"/> (Infrastructure).
    /// This class contains zero HTTP or AI-SDK code — pure business orchestration.
    /// </summary>
    public class AiService : IAiService
    {
        private readonly IUnitOfWork    _uow;
        private readonly IGeminiClient  _gemini;

        public AiService(IUnitOfWork uow, IGeminiClient gemini)
        {
            _uow    = uow;
            _gemini = gemini;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Trip Planning
        // ─────────────────────────────────────────────────────────────────────

        public async Task<GeneratedItineraryDto> GenerateTripAsync(
            string userId, AiTripRequestDto request)
        {
            // 1. Persist the request log so we can track AI usage and
            //    later link it to a saved Trip if the user chooses to save.
            var reqRepo = _uow.GetRepository<AiTripRequest, int>();
            var reqEntity = new AiTripRequest
            {
                UserId       = userId,
                Destination  = request.Destination,
                TravelDate   = request.TravelDate,
                TravelerCount = request.TravelerCount,
                BudgetTier   = Enum.TryParse<BudgetTier>(request.BudgetTier, true, out var bt)
                                    ? bt : BudgetTier.Comfort,
                Interests    = string.Join("|", request.Interests),
                RequestedAt  = DateTime.UtcNow
            };
            reqRepo.Add(reqEntity);
            await _uow.SaveChangesAsync();

            // 2. Delegate to Gemini — real AI call replaces the old stub
            var itinerary = await _gemini.GenerateTripAsync(request);
            return itinerary;
        }

        public async Task<int> SaveGeneratedTripAsync(
            string userId, GeneratedItineraryDto itinerary)
        {
            // Resolve the TouristProfile for this Identity user
            var profileRepo = _uow.GetRepository<TouristProfile, int>();
            var profiles    = await profileRepo.GetAllAsync();
            var profile     = profiles.FirstOrDefault(p => p.UserId == userId)
                              ?? throw new Exception("Tourist profile not found.");

            var tripRepo = _uow.GetRepository<Trip, int>();
            var trip = new Trip
            {
                Name                     = $"{itinerary.Destination} Trip",
                StartDate                = itinerary.StartDate,
                EndDate                  = itinerary.EndDate,
                TravelerCount            = itinerary.TravelerCount,
                EstimatedBudgetPerPerson = itinerary.EstimatedBudgetPerPerson,
                CoverImageUrl            = itinerary.CoverImageUrl,
                IsAiGenerated            = true,
                Status                   = TripStatus.Planned,
                TouristProfileId         = profile.Id
            };
            tripRepo.Add(trip);
            await _uow.SaveChangesAsync();

            // Persist each generated day and its itinerary items
            var dayRepo  = _uow.GetRepository<TripDay, int>();
            var itemRepo = _uow.GetRepository<ItineraryItem, int>();

            foreach (var day in itinerary.Days)
            {
                var tripDay = new TripDay
                {
                    TripId    = trip.Id,
                    DayNumber = day.DayNumber,
                    Date      = day.Date,
                    DayTitle  = day.DayTitle
                };
                dayRepo.Add(tripDay);
                await _uow.SaveChangesAsync();   // flush to get TripDay.Id

                foreach (var item in day.Items)
                {
                    // Parse "HH:mm" into a TimeSpan
                    var startTime = TimeSpan.TryParse(item.Time, out var ts)
                                    ? ts : TimeSpan.Zero;

                    itemRepo.Add(new ItineraryItem
                    {
                        TripDayId     = tripDay.Id,
                        StartTime     = startTime,
                        Title         = item.Title,
                        Description   = item.Description,
                        EstimatedCost = item.EstimatedCost,
                        IconType      = item.IconType,
                        ItemType      = ItemType.Custom
                    });
                }
            }

            await _uow.SaveChangesAsync();

            // Mark the original AI request as converted
            var reqRepo = _uow.GetRepository<AiTripRequest, int>();
            var allReqs = await reqRepo.GetAllAsync();
            var latest  = allReqs
                .Where(r => r.UserId == userId && !r.WasConverted)
                .OrderByDescending(r => r.RequestedAt)
                .FirstOrDefault();

            if (latest is not null)
            {
                latest.WasConverted    = true;
                latest.GeneratedTripId = trip.Id;
                latest.UpdatedAt       = DateTime.UtcNow;
                reqRepo.Update(latest);
                await _uow.SaveChangesAsync();
            }

            return trip.Id;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Chat Assistant
        // ─────────────────────────────────────────────────────────────────────

        public async Task<ChatResponseDto> SendMessageAsync(
            string userId, SendMessageDto message)
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
                // Start a new session; title defaults to the first 60 chars of the message
                session = new AiChatSession
                {
                    UserId   = userId,
                    IsActive = true,
                    Title    = message.Content.Length > 60
                               ? message.Content[..60] + "…"
                               : message.Content
                };
                sessionRepo.Add(session);
                await _uow.SaveChangesAsync();   // flush to get Session.Id
            }

            // Persist the user's message first
            var msgRepo = _uow.GetRepository<AiChatMessage, int>();
            msgRepo.Add(new AiChatMessage
            {
                SessionId = session.Id,
                Role      = AiMessageRole.User,
                Content   = message.Content,
                Timestamp = DateTime.UtcNow
            });
            await _uow.SaveChangesAsync();

            // Fetch conversation history to send as context to Gemini
            var allMessages = await msgRepo.GetAllAsync();
            var history = allMessages
                .Where(m => m.SessionId == session.Id)
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageDto
                {
                    Role      = m.Role.ToString(),
                    Content   = m.Content,
                    Timestamp = m.Timestamp
                })
                .ToList();

            // Delegate to Gemini — real AI call replaces the old stub
            var reply = await _gemini.SendChatMessageAsync(message.Content, history);

            // Persist the assistant's reply
            msgRepo.Add(new AiChatMessage
            {
                SessionId = session.Id,
                Role      = AiMessageRole.Assistant,
                Content   = reply,
                Timestamp = DateTime.UtcNow
            });
            await _uow.SaveChangesAsync();

            return new ChatResponseDto
            {
                SessionId = session.Id,
                Reply     = reply,
                Timestamp = DateTime.UtcNow
            };
        }

        public async Task<IReadOnlyList<ChatMessageDto>> GetChatHistoryAsync(
            string userId, int sessionId)
        {
            var msgRepo = _uow.GetRepository<AiChatMessage, int>();
            var all     = await msgRepo.GetAllAsync();
            return all
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.Timestamp)
                .Select(m => new ChatMessageDto
                {
                    Role      = m.Role.ToString(),
                    Content   = m.Content,
                    Timestamp = m.Timestamp
                })
                .ToList();
        }

        // ─────────────────────────────────────────────────────────────────────
        // User Preferences
        // ─────────────────────────────────────────────────────────────────────

        public async Task<UserPreferenceDto> GetPreferencesAsync(string userId)
        {
            var repo = _uow.GetRepository<UserPreference, int>();
            var all  = await repo.GetAllAsync();
            var pref = all.FirstOrDefault(p => p.UserId == userId);
            return pref is null ? new UserPreferenceDto() : MapPrefToDto(pref);
        }

        public async Task<UserPreferenceDto> UpdatePreferencesAsync(
            string userId, UserPreferenceDto dto)
        {
            var repo = _uow.GetRepository<UserPreference, int>();
            var all  = await repo.GetAllAsync();
            var pref = all.FirstOrDefault(p => p.UserId == userId);

            if (pref is null)
            {
                // ── السجل مش موجود → إنشاء جديد تماماً ──────────────────────
                // نبني الكيان بالبيانات الجديدة مباشرة بدون repo.Update
                // EF هيتعامل معاه كـ Added وهيولد الـ Id تلقائياً
                var newPref = new UserPreference
                {
                    UserId              = userId,
                    PreferredCategories = dto.PreferredCategories,
                    PreferredBudgetTier = dto.PreferredBudgetTier,
                    TravelStyle         = dto.TravelStyle,
                    MaxDailyActivities  = dto.MaxDailyActivities,
                    TravelGroupType     = Enum.TryParse<TravelGroupType>(
                                            dto.TravelGroupType, true, out var tgtNew)
                                          ? tgtNew : TravelGroupType.Solo
                };
                repo.Add(newPref);
                await _uow.SaveChangesAsync();
                return MapPrefToDto(newPref);
            }

            // ── السجل موجود بالفعل → تحديث الكيان المجلوب من الداتابيز ────────
            // الـ pref هنا مجلوب من GetAllAsync أي EF شايله بالفعل في الـ ChangeTracker
            // بـ state = Unchanged، فلما نعدل فيه وبعدين نستدعي Update
            // هيتحول state إلى Modified والـ Id حقيقي → مفيش إيرور
            pref.PreferredCategories = dto.PreferredCategories;
            pref.PreferredBudgetTier = dto.PreferredBudgetTier;
            pref.TravelStyle         = dto.TravelStyle;
            pref.MaxDailyActivities  = dto.MaxDailyActivities;
            if (Enum.TryParse<TravelGroupType>(dto.TravelGroupType, true, out var tgt))
                pref.TravelGroupType = tgt;
            pref.UpdatedAt = DateTime.UtcNow;

            repo.Update(pref);
            await _uow.SaveChangesAsync();

            return MapPrefToDto(pref);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helpers
        // ─────────────────────────────────────────────────────────────────────

        private static UserPreferenceDto MapPrefToDto(UserPreference p) => new()
        {
            PreferredCategories = p.PreferredCategories,
            PreferredBudgetTier = p.PreferredBudgetTier,
            TravelStyle         = p.TravelStyle,
            TravelGroupType     = p.TravelGroupType.ToString(),
            MaxDailyActivities  = p.MaxDailyActivities
        };
    }
}
