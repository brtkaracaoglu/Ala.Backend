using Ala.Backend.Application.Common.Enums;
using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity
{
    public interface IUserSessionRepository
    {
        Task<UserSession?> GetByFamilyIdAsync(
            Guid familyId,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSession>> GetByUserIdAsync(
            int userId,
            SessionFilterType filter,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSession>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default);

        Task<UserSession?> GetByIdAsync(
            long sessionId,
            CancellationToken cancellationToken = default);

        Task<int> DeleteInactiveOlderThanAsync(
            DateTime cutoffUtc,
            CancellationToken cancellationToken = default);
    }
}