using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Common.Models;
using UAParser;

namespace Ala.Backend.Infrastructure.Services.Sessions
{
    public sealed class UaClientInfoParser : IClientInfoParser
    {
        private readonly Parser _parser;

        public UaClientInfoParser()
        {
            _parser = Parser.GetDefault();
        }

        public ParsedClientInfo Parse(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return new ParsedClientInfo();
            }

            var parsed = _parser.Parse(userAgent);

            var browser = parsed.UA?.Family ?? "Unknown";
            var platform = parsed.OS?.Family ?? "Unknown";
            var device = ResolveDevice(parsed);
            var displayName = BuildDisplayName(browser, platform, device);

            return new ParsedClientInfo
            {
                Browser = browser,
                Platform = platform,
                Device = device,
                DisplayName = displayName
            };
        }

        private static string ResolveDevice(UAParser.ClientInfo parsed)
        {
            var family = parsed.Device?.Family;

            if (string.IsNullOrWhiteSpace(family) ||
                family.Equals("Other", StringComparison.OrdinalIgnoreCase))
            {
                return "Desktop";
            }

            return family;
        }

        private static string BuildDisplayName(string browser, string platform, string device)
        {
            if (!string.Equals(device, "Desktop", StringComparison.OrdinalIgnoreCase))
            {
                return $"{device} · {browser} · {platform}";
            }

            return $"{browser} · {platform}";
        }
    }
}