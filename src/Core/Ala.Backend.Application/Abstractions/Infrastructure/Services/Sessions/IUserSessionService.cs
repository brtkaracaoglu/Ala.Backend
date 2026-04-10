using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Enums;
using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions
{
    public interface IUserSessionService
    {
        Task CreateAsync(
            int userId,
            Guid familyId,
            IRequestContext requestContext,
            CancellationToken cancellationToken = default);

        Task TouchAsync(
            Guid familyId,
            CancellationToken cancellationToken = default);

        Task RevokeByFamilyIdAsync(
            Guid familyId,
            string revokedByIp,
            string reason,
            CancellationToken cancellationToken = default);

        Task RevokeAllAsync(
            int userId,
            string revokedByIp,
            string reason,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<UserSession>> GetUserSessionsAsync(
            int userId,
            SessionFilterType filter,
            CancellationToken cancellationToken = default);
    }
}