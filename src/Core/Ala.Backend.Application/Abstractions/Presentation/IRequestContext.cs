
namespace Ala.Backend.Application.Abstractions.Presentation
{
    public interface IRequestContext
    {
        string CorrelationId { get; }
        int? UserId { get; }
        string? Username { get; }
        string IpAddress { get; }
        string? UserAgent { get; }
    }
}
