using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Domain.Identity;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Repositories.Identity
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly MainDbContext _context;

        public RefreshTokenRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken?> GetByTokenHashWithUserAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(
            int userId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            return await _context.RefreshTokens
                .Where(x => x.UserId == userId
                            && x.UsedAtUtc == null
                            && x.RevokedAtUtc == null
                            && x.ExpiresAtUtc > now)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByFamilyIdAsync(
            Guid familyId,
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            return await _context.RefreshTokens
                .Where(x => x.FamilyId == familyId
                            && x.UsedAtUtc == null
                            && x.RevokedAtUtc == null
                            && x.ExpiresAtUtc > now)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> DeleteExpiredOrRevokedOlderThanAsync(
            DateTime cutoffUtc,
            CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Where(x =>
                    (x.RevokedAtUtc.HasValue && x.RevokedAtUtc.Value < cutoffUtc) ||
                    x.ExpiresAtUtc < cutoffUtc)
                .ExecuteDeleteAsync(cancellationToken);
        }
    }
}