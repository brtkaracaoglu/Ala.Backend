using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Infrastructure.Services.Token
{
    public class TokenRevocationService : ITokenRevocationService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenHasher _refreshTokenHasher;

        public TokenRevocationService(
            IRefreshTokenHasher refreshTokenHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenHasher = refreshTokenHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task RevokeAsync(
            string refreshToken,
            string reason,
            string revokedByIp,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var revokeIp = string.IsNullOrWhiteSpace(revokedByIp)
                ? "N/A"
                : revokedByIp;

            var tokenHash = _refreshTokenHasher.Hash(refreshToken);

            var entity = await _refreshTokenRepository.GetByTokenHashAsync(
                tokenHash,
                cancellationToken);

            if (entity is null)
                return;

            if (!entity.IsRevoked && !entity.IsUsed && !entity.IsExpired)
            {
                entity.Revoke(
                    replacedByTokenHash: null,
                    ipAddress: revokeIp,
                    reason: reason);

                _unitOfWork.WriteRepository<RefreshToken, long>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RevokeAllAsync(
            int userId,
            string reason,
            string revokedByIp,
            CancellationToken cancellationToken = default)
        {
            var revokeIp = string.IsNullOrWhiteSpace(revokedByIp)
                ? "N/A"
                : revokedByIp;

            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(
                userId,
                cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke(null, revokeIp, reason);
            }

            _unitOfWork.WriteRepository<RefreshToken, long>().UpdateRange(activeTokens);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<Guid?> GetFamilyIdByRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            var tokenHash = _refreshTokenHasher.Hash(refreshToken);
            var entity = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

            return entity?.FamilyId;
        }

        public async Task RevokeByFamilyIdAsync(
            Guid familyId,
            string reason,
            string revokedByIp,
            CancellationToken cancellationToken = default)
        {
            var revokeIp = string.IsNullOrWhiteSpace(revokedByIp)
                ? "N/A"
                : revokedByIp;

            var activeTokens = await _refreshTokenRepository.GetActiveByFamilyIdAsync(
                familyId,
                cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke(null, revokeIp, reason);
            }

            _unitOfWork.WriteRepository<RefreshToken, long>().UpdateRange(activeTokens);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
