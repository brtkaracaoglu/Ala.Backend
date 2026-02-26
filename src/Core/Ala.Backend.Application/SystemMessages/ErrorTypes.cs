
namespace Ala.Backend.Application.SystemMessages
{
    public static class ErrorTypes
    {
        private const string BaseUrl = "https://ala-backend.com/errors";

        public const string BadRequest = $"{BaseUrl}/bad-request";
        public const string Validation = $"{BaseUrl}/validation";
        public const string NotFound = $"{BaseUrl}/not-found";
        public const string Conflict = $"{BaseUrl}/conflict";
        public const string Unauthorized = $"{BaseUrl}/unauthorized";
        public const string Forbidden = $"{BaseUrl}/forbidden";
        public const string BusinessRule = $"{BaseUrl}/business-rule";
        public const string Database = $"{BaseUrl}/database";
        public const string Integration = $"{BaseUrl}/integration";
        public const string ServiceUnavailable = $"{BaseUrl}/service-unavailable";
        public const string Timeout = $"{BaseUrl}/timeout";
        public const string Internal = $"{BaseUrl}/internal";
    }
}
