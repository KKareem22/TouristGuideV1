using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using TouristGuide.API.Models;

namespace TouristGuide.API.Middleware
{
    /// <summary>
    /// .NET 8 IExceptionHandler implementation.
    /// Catches every unhandled exception that bubbles up through the pipeline
    /// and writes a consistent <see cref="ErrorResponse"/> JSON payload so the
    /// Flutter app always receives the same parseable shape regardless of the
    /// exception type or origin layer.
    ///
    /// Registration: services.AddExceptionHandler&lt;GlobalExceptionHandler&gt;()
    ///               app.UseExceptionHandler()
    /// </summary>
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment                _env;

        // Serialise with camelCase so Flutter/Dart receives statusCode, message, details
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger,
            IHostEnvironment                env)
        {
            _logger = logger;
            _env    = env;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext          httpContext,
            Exception            exception,
            CancellationToken    cancellationToken)
        {
            // Log the full exception at Error level so it appears in the server logs
            _logger.LogError(
                exception,
                "Unhandled exception on {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);

            // Map well-known exception types to meaningful HTTP status codes
            var (statusCode, message) = exception switch
            {
                KeyNotFoundException                => (HttpStatusCode.NotFound,           "Resource not found."),
                UnauthorizedAccessException         => (HttpStatusCode.Unauthorized,       "Unauthorised access."),
                ArgumentException or ArgumentNullException
                                                    => (HttpStatusCode.BadRequest,         "Invalid argument."),
                InvalidOperationException           => (HttpStatusCode.UnprocessableEntity,"Invalid operation."),
                HttpRequestException httpEx         => (httpEx.StatusCode ?? HttpStatusCode.BadGateway,
                                                        "Upstream service error."),
                _                                  => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
            };

            // In Production hide internal details to avoid information disclosure;
            // in Development expose the full message and stack trace for easier debugging.
            var details = _env.IsDevelopment()
                ? $"{exception.Message}\n\n{exception.StackTrace}"
                : exception.Message;

            var response = new ErrorResponse
            {
                StatusCode = (int)statusCode,
                Message    = message,
                Details    = details
            };

            httpContext.Response.StatusCode  = (int)statusCode;
            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsync(
                JsonSerializer.Serialize(response, _jsonOptions),
                cancellationToken);

            // Return true → exception has been handled, pipeline stops here
            return true;
        }
    }
}
