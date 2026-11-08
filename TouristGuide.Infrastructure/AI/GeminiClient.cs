using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TouristGuide.Application.DTOs.AI;
using TouristGuide.Application.Interfaces;
using TouristGuide.Application.Models;

namespace TouristGuide.Infrastructure.AI
{
    /// <summary>
    /// Calls the Google Gemini REST API (v1beta / gemini-1.5-flash).
    /// All prompt engineering and JSON parsing lives here so that the
    /// Application layer never sees raw HTTP or Gemini-specific shapes.
    /// </summary>
    public class GeminiClient : IGeminiClient
    {
        private readonly HttpClient      _http;
        private readonly GeminiSettings  _settings;
        private readonly ILogger<GeminiClient> _logger;

        // Shared, case-insensitive JSON options used for both serialising requests
        // and deserialising Gemini responses.
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive    = true,
            DefaultIgnoreCondition         = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy           = JsonNamingPolicy.CamelCase
        };

        public GeminiClient(
            HttpClient              http,
            IOptions<GeminiSettings> settings,
            ILogger<GeminiClient>   logger)
        {
            _http     = http;
            _settings = settings.Value;
            _logger   = logger;
        }

        // ─────────────────────────────────────────────────────────────────────
        // Public contract methods
        // ─────────────────────────────────────────────────────────────────────

        public async Task<GeneratedItineraryDto> GenerateTripAsync(
            AiTripRequestDto request,
            CancellationToken ct = default)
        {
            var prompt = BuildTripPlanningPrompt(request);
            var rawJson = await CallGeminiAsync(prompt, ct);

            // Gemini is asked to return strict JSON; parse it directly.
            return ParseItineraryFromJson(rawJson, request);
        }

        public async Task<string> SendChatMessageAsync(
            string userMessage,
            IReadOnlyList<ChatMessageDto> history,
            CancellationToken ct = default)
        {
            var prompt = BuildChatPrompt(userMessage, history);
            return await CallGeminiAsync(prompt, ct);
        }

        // ─────────────────────────────────────────────────────────────────────
        // Core HTTP call
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Posts a single user prompt to Gemini and returns the model's
        /// first candidate text, with whitespace trimmed.
        /// </summary>
        private async Task<string> CallGeminiAsync(string prompt, CancellationToken ct)
        {
            // Build the Gemini generateContent request body
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                },
                generationConfig = new
                {
                    temperature     = 0.7,
                    maxOutputTokens = 4096
                }
            };

            // Append the API key as a query parameter (Gemini REST convention)
            var url = $"{_settings.BaseUrl}?key={_settings.ApiKey}";

            _logger.LogDebug("Calling Gemini API at {Url}", _settings.BaseUrl);

            using var response = await _http.PostAsJsonAsync(url, requestBody, _jsonOptions, ct);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError(
                    "Gemini API returned {StatusCode}: {ErrorBody}",
                    (int)response.StatusCode, errorBody);

                throw new HttpRequestException(
                    $"Gemini API error {(int)response.StatusCode}: {errorBody}");
            }

            // Deserialise into our internal response shape
            var geminiResponse = await response.Content
                .ReadFromJsonAsync<GeminiApiResponse>(_jsonOptions, ct);

            var text = geminiResponse
                ?.Candidates?.FirstOrDefault()
                ?.Content?.Parts?.FirstOrDefault()
                ?.Text;

            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("Gemini returned an empty response.");
                throw new InvalidOperationException(
                    "Gemini returned an empty response. Please try again.");
            }

            return text.Trim();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Prompt builders
        // ─────────────────────────────────────────────────────────────────────

        private static string BuildTripPlanningPrompt(AiTripRequestDto request)
        {
            var interests = request.Interests.Any()
                ? string.Join(", ", request.Interests)
                : "general sightseeing";

            // The number of days is derived from today to the travel date, capped
            // so the model produces a sensible short trip (3–7 days).
            var startDate = request.TravelDate.Date;
            var days      = Math.Clamp(request.TravelerCount > 1 ? 4 : 3, 3, 7);
            var endDate   = startDate.AddDays(days - 1);

            // We instruct the model to return ONLY valid JSON that matches our
            // GeneratedItineraryDto shape.  No prose, no markdown fences.
            // Using StringBuilder to avoid raw-string-literal brace-escaping issues.
            var p = new StringBuilder();
            p.AppendLine($"You are an expert travel planner. Generate a detailed {days}-day itinerary for the following trip.");
            p.AppendLine("Respond ONLY with a single valid JSON object — no markdown, no explanation, no code fences.");
            p.AppendLine();
            p.AppendLine("Trip details:");
            p.AppendLine($"- Destination  : {request.Destination}");
            p.AppendLine($"- Start date   : {startDate:yyyy-MM-dd}");
            p.AppendLine($"- End date     : {endDate:yyyy-MM-dd}");
            p.AppendLine($"- Travellers   : {request.TravelerCount}");
            p.AppendLine($"- Budget tier  : {request.BudgetTier}   (Easygoing = budget, Comfort = mid-range, Luxury = premium)");
            p.AppendLine($"- Interests    : {interests}");
            p.AppendLine();
            p.AppendLine("Required JSON schema (adhere strictly — all fields are required):");
            p.AppendLine("{");
            p.AppendLine("  \"destination\": \"string\",");
            p.AppendLine("  \"startDate\":   \"yyyy-MM-ddTHH:mm:ss\",");
            p.AppendLine("  \"endDate\":     \"yyyy-MM-ddTHH:mm:ss\",");
            p.AppendLine("  \"travelerCount\": number,");
            p.AppendLine("  \"estimatedBudgetPerPerson\": \"~$NNN/person\",");
            p.AppendLine("  \"days\": [");
            p.AppendLine("    {");
            p.AppendLine("      \"dayNumber\": 1,");
            p.AppendLine("      \"date\": \"yyyy-MM-ddTHH:mm:ss\",");
            p.AppendLine("      \"dayTitle\": \"string — one evocative phrase\",");
            p.AppendLine("      \"items\": [");
            p.AppendLine("        {");
            p.AppendLine("          \"time\":          \"HH:mm\",");
            p.AppendLine("          \"title\":         \"string\",");
            p.AppendLine("          \"description\":   \"string\",");
            p.AppendLine("          \"estimatedCost\": number,");
            p.AppendLine("          \"iconType\":      \"car | fork | compass | camera | hotel | coffee | walk | boat | museum\"");
            p.AppendLine("        }");
            p.AppendLine("      ]");
            p.AppendLine("    }");
            p.AppendLine("  ]");
            p.AppendLine("}");
            p.AppendLine();
            p.AppendLine($"Generate {days} days of items, at least 3 items per day.");
            p.AppendLine($"Make every item specific to {request.Destination} and relevant to the interests: {interests}.");
            return p.ToString();
        }

        private static string BuildChatPrompt(
            string userMessage,
            IReadOnlyList<ChatMessageDto> history)
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "You are Roam, a friendly and knowledgeable AI travel assistant. " +
                "You give concise, practical, personalised travel advice. " +
                "Keep replies under 200 words unless asked for detail.");
            sb.AppendLine();

            // Include the conversation history so Gemini has context
            if (history.Count > 0)
            {
                sb.AppendLine("Conversation so far:");
                foreach (var msg in history.TakeLast(10)) // limit context window
                {
                    var role = msg.Role == "User" ? "Traveller" : "Roam";
                    sb.AppendLine($"{role}: {msg.Content}");
                }
                sb.AppendLine();
            }

            sb.AppendLine($"Traveller: {userMessage}");
            sb.AppendLine("Roam:");

            return sb.ToString();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Itinerary JSON parser
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Parses the raw JSON text returned by Gemini into a
        /// <see cref="GeneratedItineraryDto"/>. Falls back gracefully when
        /// the model returns valid but slightly non-conforming JSON.
        /// </summary>
        private GeneratedItineraryDto ParseItineraryFromJson(
            string rawJson, AiTripRequestDto request)
        {
            // Strip possible markdown code fences the model might still include
            var json = rawJson
                .Trim()
                .TrimStart('`')
                .TrimEnd('`');

            if (json.StartsWith("json", StringComparison.OrdinalIgnoreCase))
                json = json[4..].TrimStart();

            try
            {
                var parsed = JsonSerializer.Deserialize<GeminiItineraryResponse>(
                    json, _jsonOptions);

                if (parsed is null)
                    throw new JsonException("Deserialized object was null.");

                // Map from the Gemini-response shape to our application DTO
                return new GeneratedItineraryDto
                {
                    Destination              = parsed.Destination ?? request.Destination,
                    StartDate                = parsed.StartDate == default
                                               ? request.TravelDate
                                               : parsed.StartDate,
                    EndDate                  = parsed.EndDate == default
                                               ? request.TravelDate.AddDays(3)
                                               : parsed.EndDate,
                    TravelerCount            = parsed.TravelerCount > 0
                                               ? parsed.TravelerCount
                                               : request.TravelerCount,
                    EstimatedBudgetPerPerson = parsed.EstimatedBudgetPerPerson
                                               ?? "~$200/person",
                    Days = (parsed.Days ?? []).Select((d, idx) => new GeneratedDayDto
                    {
                        DayNumber = d.DayNumber > 0 ? d.DayNumber : idx + 1,
                        Date      = d.Date == default
                                    ? request.TravelDate.AddDays(idx)
                                    : d.Date,
                        DayTitle  = d.DayTitle,
                        Items     = (d.Items ?? []).Select(i => new GeneratedItemDto
                        {
                            Time          = i.Time          ?? "09:00",
                            Title         = i.Title         ?? string.Empty,
                            Description   = i.Description,
                            EstimatedCost = i.EstimatedCost,
                            IconType      = i.IconType
                        }).ToList()
                    }).ToList()
                };
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "Failed to parse Gemini itinerary JSON. Raw response length={Length}",
                    rawJson.Length);

                throw new InvalidOperationException(
                    "The AI returned a response in an unexpected format. Please try again.", ex);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Private Gemini API response shapes
        // These are internal — they never cross the Application layer boundary.
        // ─────────────────────────────────────────────────────────────────────

        private sealed class GeminiApiResponse
        {
            public List<GeminiCandidate>? Candidates { get; set; }
        }

        private sealed class GeminiCandidate
        {
            public GeminiContent? Content { get; set; }
        }

        private sealed class GeminiContent
        {
            public List<GeminiPart>? Parts { get; set; }
        }

        private sealed class GeminiPart
        {
            public string? Text { get; set; }
        }

        // Mirrors the JSON schema we ask the model to produce for itineraries
        private sealed class GeminiItineraryResponse
        {
            public string?           Destination              { get; set; }
            public DateTime          StartDate                { get; set; }
            public DateTime          EndDate                  { get; set; }
            public int               TravelerCount            { get; set; }
            public string?           EstimatedBudgetPerPerson { get; set; }
            public List<GeminiDay>?  Days                     { get; set; }
        }

        private sealed class GeminiDay
        {
            public int              DayNumber { get; set; }
            public DateTime         Date      { get; set; }
            public string?          DayTitle  { get; set; }
            public List<GeminiItem>? Items    { get; set; }
        }

        private sealed class GeminiItem
        {
            public string?   Time          { get; set; }
            public string?   Title         { get; set; }
            public string?   Description   { get; set; }
            public decimal?  EstimatedCost { get; set; }
            public string?   IconType      { get; set; }
        }
    }
}
