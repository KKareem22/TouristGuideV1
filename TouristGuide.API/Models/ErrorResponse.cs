namespace TouristGuide.API.Models
{
    /// <summary>
    /// Unified error envelope returned by the Global Exception Handler.
    /// Flutter can always deserialise { statusCode, message, details } regardless
    /// of which exception was thrown or which layer it came from.
    /// </summary>
    public sealed class ErrorResponse
    {
        public int    StatusCode { get; init; }
        public string Message   { get; init; } = string.Empty;
        public string Details   { get; init; } = string.Empty;
    }
}
