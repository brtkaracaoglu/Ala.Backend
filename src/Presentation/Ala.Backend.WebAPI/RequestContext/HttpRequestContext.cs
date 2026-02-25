
using Ala.Backend.Application.Abstractions.Presentation;
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
            ?? Guid.NewGuid().ToString();

        public int? UserId =>
            int.TryParse(
                _http.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out var id)
                ? id
                : null;

        public string? Username =>
            _http.HttpContext?.User?.Identity?.Name;

        public string IpAddress =>
            _http.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            ?? "127.0.0.1";

        public string? UserAgent =>
            _http.HttpContext?.Request?.Headers["User-Agent"];
    }
}
