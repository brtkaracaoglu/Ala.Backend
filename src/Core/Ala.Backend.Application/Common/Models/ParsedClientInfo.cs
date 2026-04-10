namespace Ala.Backend.Application.Common.Models
{
    public class ParsedClientInfo
    {
        public string Browser { get; init; } = "Unknown";
        public string Platform { get; init; } = "Unknown";
        public string Device { get; init; } = "Unknown";
        public string DisplayName { get; init; } = "Unknown device";
    }
}
