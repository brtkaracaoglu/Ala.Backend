using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Enums;
using Ala.Backend.Domain.Identity;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Repositories.Identity
{
    public class UserSessionRepository : IUserSessionRepository
    {
        private readonly MainDbContext _context;

        public UserSessionRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<UserSession?> GetByFamilyIdAsync(
            Guid familyId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x => x.FamilyId == familyId, cancellationToken);
        }

        public async Task<IReadOnlyList<UserSession>> GetByUserIdAsync(
            int userId,
            SessionFilterType filter,
            CancellationToken cancellationToken = default)
        {
            IQueryable<UserSession> query = _context.UserSessions
                .Where(x => x.UserId == userId);

            query = filter switch
            {
                SessionFilterType.ActiveOnly => query.Where(x => x.RevokedAtUtc == null),
                SessionFilterType.RevokedOnly => query.Where(x => x.RevokedAtUtc != null),
                _ => query
            };

            return await query
                .OrderByDescending(x => x.LastActivityOnUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<UserSession>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .Where(x => x.UserId == userId && x.RevokedAtUtc == null)
                .OrderByDescending(x => x.LastActivityOnUtc)
                .ToListAsync(cancellationToken);
        }

        public async Task<UserSession?> GetByIdAsync(long sessionId,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
        }

        public async Task<int> DeleteInactiveOlderThanAsync(
            DateTime cutoffUtc,
            CancellationToken cancellationToken = default)
        {
            return await _context.UserSessions
                .Where(x =>
                    x.RevokedAtUtc.HasValue &&
                    (
                        x.RevokedAtUtc.Value < cutoffUtc ||
                        x.LastActivityOnUtc < cutoffUtc
                    ))
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}