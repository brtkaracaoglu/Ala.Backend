using Ala.Backend.Application.Common.Enums;
using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashWithUserAsync(
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<RefreshToken>> GetActiveByFamilyIdAsync(
            Guid familyId,
            CancellationToken cancellationToken = default);

        Task<int> DeleteExpiredOrRevokedOlderThanAsync(
            DateTime cutoffUtc,
            CancellationToken cancellationToken = default);

    }
}
