namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Token
{
    public interface ITokenRevocationService
    {
        Task RevokeAsync(
            string refreshToken,
            string reason,
            string revokedByIp,
            CancellationToken cancellationToken = default);

        Task RevokeAllAsync(
            int userId,
            string reason,
            string revokedByIp,
            CancellationToken cancellationToken = default);

        Task<Guid?> GetFamilyIdByRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);

        Task RevokeByFamilyIdAsync(
            Guid familyId,
            string reason,
            string revokedByIp,
            CancellationToken cancellationToken = default);
    }
}
