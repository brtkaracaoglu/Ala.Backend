using Ala.Backend.Application.SystemMessages;

namespace Ala.Backend.Application.Common.Exceptions
{
    public abstract class AppException : Exception
    {
        public string TypeUri { get; }
        public int Status { get; }
        public string Title { get; }
        public string Detail => Message;
        public string? Instance { get; set; }
        public string? CorrelationId { get; set; }

        // Genel hatalar için liste (Validation hariç)
        public IEnumerable<string>? Errors { get; protected set; }

        protected AppException(string title, string detail, int status, string typeUri)
            : base(detail)
        {
            Title = title;
            Status = status;
            TypeUri = typeUri;
        }
    }

    // 400 — Bad Request (Genel Format Hataları)
    public class BadRequestException : AppException
    {
        public BadRequestException(string detail)
            : base("Bad Request", detail, 400, ErrorTypes.BadRequest) { }
    }

    // 400 — Validation (FluentValidation & Identity)
    public class AppValidationException : AppException
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
    public class NotFoundException : AppException
    {
        public NotFoundException(string detail)
            : base("Not Found", detail, 404, ErrorTypes.NotFound) { }
    }

    // 409 — Conflict (Opsiyonel PropertyName Eklendi)
    public class ConflictException : AppException
    {
        public string? PropertyName { get; }

        public ConflictException(string detail, string? propertyName = null)
            : base("Conflict", detail, 409, ErrorTypes.Conflict)
        {
            PropertyName = propertyName;
        }
    }

    // 401 — Unauthorized
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string detail = "Unauthorized access.")
            : base("Unauthorized", detail, 401, ErrorTypes.Unauthorized) { }
    }

    // 403 — Forbidden
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string detail = "Forbidden.")
            : base("Forbidden", detail, 403, ErrorTypes.Forbidden) { }
    }

    // 422 — Business Rule (Opsiyonel PropertyName Eklendi)
    public class BusinessRuleException : AppException
    {
        public string? PropertyName { get; }

        public BusinessRuleException(string detail, string? propertyName = null)
            : base("Business Rule Violation", detail, 422, ErrorTypes.BusinessRule)
        {
            PropertyName = propertyName;
        }
    }

    // 500 — Database
    public class DatabaseException : AppException
    {
        public DatabaseException(string detail)
            : base("Database Error", detail, 500, ErrorTypes.Database) { }
    }

    // 500 — Internal Server Error (EKLENDİ)
    // Sunucu taraflı konfigürasyon, dosya okuma vb. hatalar için.
    public class InternalServerException : AppException
    {
        public InternalServerException(string detail)
            : base("Internal Server Error", detail, 500, ErrorTypes.Internal) { }
    }

    // 502 — Integration
    public class IntegrationException : AppException
    {
        public IntegrationException(string detail)
            : base("Integration Error", detail, 502, ErrorTypes.Integration) { }
    }

    // 503 — Service Unavailable
    public class ServiceUnavailableException : AppException
    {
        public ServiceUnavailableException(string detail)
            : base("Service Unavailable", detail, 503, ErrorTypes.ServiceUnavailable) { }
    }

    // 504 — Timeout
    public class TimeoutAppException : AppException
    {
        public TimeoutAppException(string detail = "Operation timed out.")
            : base("Timeout", detail, 504, ErrorTypes.Timeout) { }
    }

    // 500 — General Failure
    public class OperationFailedException : AppException
    {
        public OperationFailedException(string detail)
            : base("Operation Failed", detail, 500, ErrorTypes.Internal) { }
    }
}
