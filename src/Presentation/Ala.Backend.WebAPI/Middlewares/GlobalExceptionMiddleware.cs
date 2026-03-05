using Ala.Backend.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Middlewares
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            // Swagger veya Scalar gibi dökümantasyon araçlarını middleware dışında tut
            if (context.Request.Path.StartsWithSegments("/swagger") || context.Request.Path.StartsWithSegments("/scalar"))
            {
                await _next(context);
                return;
            }

            // Not: correlationId artık Serilog LogContext üzerinden otomatik takip ediliyor.
            var correlationId = context.Items["CorrelationId"]?.ToString();

            try
            {
                await _next(context);
            }
            catch (AppValidationException ex)
            {
                LogWarning(context, ex, "Validation error");
                await WriteProblemAsync(context, CreateValidationProblem(context, ex, correlationId));
            }
            catch (AppException ex)
            {
                LogWarning(context, ex, "Application error");
                await WriteProblemAsync(context, CreateApplicationProblem(context, ex, correlationId));
            }
            catch (Exception ex)
            {
                LogError(context, ex);
                await WriteProblemAsync(context, CreateUnhandledProblem(context, ex, correlationId));
            }
        }

        private ProblemDetails CreateValidationProblem(HttpContext context, AppValidationException ex, string? correlationId)
        {
            var pd = CreateBaseProblemDetails(context, ex, correlationId);
            pd.Extensions["errors"] = ex.ValidationErrors;
            return pd;
        }

        private ProblemDetails CreateApplicationProblem(HttpContext context, AppException ex, string? correlationId)
        {
            var pd = CreateBaseProblemDetails(context, ex, correlationId);

            if (ex is BusinessRuleException busEx && !string.IsNullOrEmpty(busEx.PropertyName))
            {
                pd.Extensions["errors"] = new Dictionary<string, string[]> { { busEx.PropertyName, new[] { ex.Detail } } };
            }
            else if (ex.Errors?.Any() == true)
            {
                pd.Extensions["errors"] = new Dictionary<string, IEnumerable<string>> { { "General", ex.Errors } };
            }

            return pd;
        }

        private ProblemDetails CreateUnhandledProblem(HttpContext context, Exception ex, string? correlationId)
        {
            var pd = new ProblemDetails
            {
                Title = "Internal Server Error",
                Detail = "Beklenmeyen bir hata oluştu.",
                Type = "https://ala-backend.com/errors/internal",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.Path
            };

            AddCommonExtensions(context, pd, correlationId);

            if (_env.IsDevelopment())
            {
                pd.Extensions["exceptionMessage"] = ex.Message;
                pd.Extensions["stackTrace"] = ex.StackTrace;
            }

            return pd;
        }

        private static ProblemDetails CreateBaseProblemDetails(HttpContext context, AppException ex, string? correlationId)
        {
            var pd = new ProblemDetails
            {
                Title = ex.Title,
                Detail = ex.Detail,
                Type = ex.TypeUri,
                Status = ex.Status,
                Instance = context.Request.Path
            };

            AddCommonExtensions(context, pd, correlationId);
            return pd;
        }

        private static void AddCommonExtensions(HttpContext context, ProblemDetails pd, string? correlationId)
        {
            // TraceId olarak öncelikle bizim correlationId'mizi, yoksa sistemin TraceIdentifier'ını kullanıyoruz.
            pd.Extensions["traceId"] = correlationId ?? context.TraceIdentifier;
            pd.Extensions["method"] = context.Request.Method;
        }


        private static async Task WriteProblemAsync(HttpContext context, ProblemDetails pd)
        {
            if (context.Response.HasStarted) return;

            context.Response.StatusCode = pd.Status ?? StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(pd);
        }

        // Serilog Structured Logging kullanarak loglama metodları
        private void LogWarning(HttpContext context, Exception ex, string title)
        {
            _logger.LogWarning(ex, "{Title} | Path: {Path} | Method: {Method}",
                title, context.Request.Path, context.Request.Method);
        }

        private void LogError(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred | Path: {Path} | Method: {Method}",
                context.Request.Path, context.Request.Method);
        }
    }
}