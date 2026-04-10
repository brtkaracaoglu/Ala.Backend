using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Enums;
using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Infrastructure.Services.Sessions
{
    public class UserSessionService : IUserSessionService
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserSessionService(
            IUserSessionRepository userSessionRepository,
            IUnitOfWork unitOfWork)
        {
            _userSessionRepository = userSessionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task CreateAsync(
            int userId,
            Guid familyId,
            IRequestContext requestContext,
            CancellationToken cancellationToken = default)
        {
            var existingSession = await _userSessionRepository.GetByFamilyIdAsync(familyId, cancellationToken);
            if (existingSession is not null)
                return;

            var ip = string.IsNullOrWhiteSpace(requestContext.IpAddress) ? "N/A" : requestContext.IpAddress;
            var userAgent = string.IsNullOrWhiteSpace(requestContext.UserAgent) ? "Unknown" : requestContext.UserAgent;

            var session = new UserSession
            {
                UserId = userId,
                FamilyId = familyId,
                CreatedByIp = ip,
                CreatedByUserAgent = userAgent,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _unitOfWork.WriteRepository<UserSession, long>()
                .AddAsync(session, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task TouchAsync(
            Guid familyId,
            CancellationToken cancellationToken = default)
        {
            var session = await _userSessionRepository.GetByFamilyIdAsync(familyId, cancellationToken);
            if (session is null || session.IsRevoked)
                return;

            session.Touch();

            _unitOfWork.WriteRepository<UserSession, long>().Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeByFamilyIdAsync(
            Guid familyId,
            string revokedByIp,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var session = await _userSessionRepository.GetByFamilyIdAsync(familyId, cancellationToken);
            if (session is null || session.IsRevoked)
                return;

            session.Revoke(
                string.IsNullOrWhiteSpace(revokedByIp) ? "N/A" : revokedByIp,
                reason);

            _unitOfWork.WriteRepository<UserSession, long>().Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeAllAsync(
            int userId,
            string revokedByIp,
            string reason,
            CancellationToken cancellationToken = default)
        {
            var sessions = await _userSessionRepository.GetActiveByUserIdAsync(userId, cancellationToken);

            var ip = string.IsNullOrWhiteSpace(revokedByIp) ? "N/A" : revokedByIp;

            foreach (var session in sessions)
            {
                session.Revoke(ip, reason);
            }

            _unitOfWork.WriteRepository<UserSession, long>().UpdateRange(sessions);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<UserSession>> GetUserSessionsAsync(
            int userId,
            SessionFilterType filter,
            CancellationToken cancellationToken = default)
        {
            return await _userSessionRepository.GetByUserIdAsync(
                userId,
                filter,
                cancellationToken);
        }
    }
}