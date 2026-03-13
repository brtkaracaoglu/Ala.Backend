using Ala.Backend.Application.SystemMessages;

namespace Ala.Backend.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public string TypeUri { get; }
        public int StatusCode { get; }
        public string Title { get; }
        public string Detail => Message;

        protected AppException(string title, string detail, int statusCode, string typeUri, Exception? innerException = null) : base(detail, innerException)
        {
            Title = title;
            StatusCode = statusCode;
            TypeUri = typeUri;
        }
    }

    // 400 — Bad Request (Genel Format Hataları)
    public sealed class BadRequestException : AppException
    {
        public BadRequestException(string detail)
            : base("Bad Request", detail, 400, ErrorTypes.BadRequest) { }
    }

    // 400 — Validation (FluentValidation & Identity)
    public sealed class AppValidationException : AppException
    {
        // Dictionary yapısı burada özel olarak tanımlı
        public IDictionary<string, string[]> ValidationErrors { get; }

        public AppValidationException(string detail, IDictionary<string, string[]> errors)
            : base("Validation Error", detail, 400, ErrorTypes.Validation)
        {
            ValidationErrors = errors;
        }

        // Tek bir hata için kolay constructor
        public AppValidationException(string propertyName, string errorMessage)
             : base("Validation Error", "Validation failed", 400, ErrorTypes.Validation)
        {
            ValidationErrors = new Dictionary<string, string[]>
            {
                { propertyName, new[] { errorMessage } }
            };
        }
    }

    // 404 — Not Found
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string detail)
            : base("Not Found", detail, 404, ErrorTypes.NotFound) { }
    }

    // 409 — Conflict (Opsiyonel PropertyName Eklendi)
    public sealed class ConflictException : AppException
    {
        public string? PropertyName { get; }

        public ConflictException(string detail, string? propertyName = null)
            : base("Conflict", detail, 409, ErrorTypes.Conflict)
        {
            PropertyName = propertyName;
        }
    }

    // 401 — Unauthorized
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string detail = "Unauthorized access.")
            : base("Unauthorized", detail, 401, ErrorTypes.Unauthorized) { }
    }

    // 403 — Forbidden
    public sealed class ForbiddenException : AppException
    {
        public ForbiddenException(string detail = "Forbidden.")
            : base("Forbidden", detail, 403, ErrorTypes.Forbidden) { }
    }

    // 422 — Business Rule (Opsiyonel PropertyName Eklendi)
    public sealed class BusinessRuleException : AppException
    {
        public string? PropertyName { get; }

        public BusinessRuleException(string detail, string? propertyName = null)
            : base("Business Rule Violation", detail, 422, ErrorTypes.BusinessRule)
        {
            PropertyName = propertyName;
        }
    }

    // 500 — Database
    public sealed class DatabaseException : AppException
    {
        public DatabaseException(string detail)
            : base("Database Error", detail, 500, ErrorTypes.Database) { }
    }

    // 500 — Internal Server Error (EKLENDİ)
    // Sunucu taraflı konfigürasyon, dosya okuma vb. hatalar için.
    public sealed class InternalServerException : AppException
    {
        public InternalServerException(string detail)
            : base("Internal Server Error", detail, 500, ErrorTypes.Internal) { }
    }

    // 502 — Integration
    public sealed class IntegrationException : AppException
    {
        public IntegrationException(string detail)
            : base("Integration Error", detail, 502, ErrorTypes.Integration) { }
    }

    // 503 — Service Unavailable
    public sealed class ServiceUnavailableException : AppException
    {
        public ServiceUnavailableException(string detail)
            : base("Service Unavailable", detail, 503, ErrorTypes.ServiceUnavailable) { }
    }

    // 504 — Timeout
    public sealed class TimeoutAppException : AppException
    {
        public TimeoutAppException(string detail = "Operation timed out.")
            : base("Timeout", detail, 504, ErrorTypes.Timeout) { }
    }

    // 500 — General Failure
    public sealed class OperationFailedException : AppException
    {
        public OperationFailedException(string detail)
            : base("Operation Failed", detail, 500, ErrorTypes.Internal) { }
    }
}
