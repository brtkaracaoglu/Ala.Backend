using Ala.Backend.Application.Abstractions.Presentation;
using System.Net;
using System.Security.Claims;

namespace Ala.Backend.WebAPI.RequestContext
{
    internal sealed class HttpRequestContext : IRequestContext
    {
        private readonly IHttpContextAccessor _http;

        public HttpRequestContext(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string CorrelationId =>
            _http.HttpContext?.Items["CorrelationId"]?.ToString()
            ?? _http.HttpContext?.TraceIdentifier
            ?? "unknown";

        public int? UserId =>
            int.TryParse(
                _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var id)
                ? id
                : null;

        public string? Username =>
            _http.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value
            ?? _http.HttpContext?.User?.Identity?.Name;

        public string? Email =>
            _http.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
            ?? _http.HttpContext?.User?.FindFirst("email")?.Value;

        public ClaimsPrincipal? User =>
            _http.HttpContext?.User;

        public string IpAddress
        {
            get
            {
                var ip = _http.HttpContext?.Connection?.RemoteIpAddress;

                if (ip is null)
                    return "127.0.0.1";

                if (IPAddress.IsLoopback(ip))
                    return "127.0.0.1";

                if (ip.IsIPv4MappedToIPv6)
                    ip = ip.MapToIPv4();

                return ip.ToString();
            }
        }

        public string? UserAgent =>
            _http.HttpContext?.Request?.Headers["User-Agent"].ToString();
    }
}