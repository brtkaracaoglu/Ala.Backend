using Ala.Backend.Application.DTOs.Maintenance;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Maintenance
{
    public interface IAuthDataCleanupService
    {
        Task<AuthCleanupResult> CleanupAsync(CancellationToken cancellationToken = default);
        Task<int> CleanupRefreshTokensAsync(CancellationToken cancellationToken = default);
        Task<int> CleanupUserSessionsAsync(CancellationToken cancellationToken = default);
    }
}