using Ala.Backend.Application.Abstractions.Infrastructure.Services.Maintenance;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.DTOs.Maintenance;
using Ala.Backend.Persistence.Main.Settings;
using Microsoft.Extensions.Options;

namespace Ala.Backend.Persistence.Main.Services.Maintenance
{
    public sealed class AuthDataCleanupService : IAuthDataCleanupService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly AuthCleanupSettings _settings;

        public AuthDataCleanupService(
            IRefreshTokenRepository refreshTokenRepository,
            IUserSessionRepository userSessionRepository,
            IOptions<AuthCleanupSettings> settings)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userSessionRepository = userSessionRepository;
            _settings = settings.Value;
        }

        public async Task<AuthCleanupResult> CleanupAsync(CancellationToken cancellationToken = default)
        {
            var deletedRefreshTokens = await CleanupRefreshTokensAsync(cancellationToken);
            var deletedUserSessions = await CleanupUserSessionsAsync(cancellationToken);

            return new AuthCleanupResult
            {
                DeletedRefreshTokens = deletedRefreshTokens,
                DeletedUserSessions = deletedUserSessions,
                ExecutedAtUtc = DateTime.UtcNow
            };
        }

        public async Task<int> CleanupRefreshTokensAsync(CancellationToken cancellationToken = default)
        {
            var cutoffUtc = DateTime.UtcNow.AddDays(-_settings.RefreshTokenRetentionDays);

            return await _refreshTokenRepository.DeleteExpiredOrRevokedOlderThanAsync(
                cutoffUtc,
                cancellationToken);
        }

        public async Task<int> CleanupUserSessionsAsync(CancellationToken cancellationToken = default)
        {
            var cutoffUtc = DateTime.UtcNow.AddDays(-_settings.UserSessionRetentionDays);

            return await _userSessionRepository.DeleteInactiveOlderThanAsync(
                cutoffUtc,
                cancellationToken);
        }
    }
}