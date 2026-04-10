using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Sessions.RevokeSession
{
    public class RevokeSessionCommandHandler
        : IRequestHandler<RevokeSessionCommandRequest, SuccessDetails<RevokeSessionResponseDto>>
    {
        private readonly IRequestContext _requestContext;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUserSessionService _userSessionService;
        private readonly ITokenRevocationService _tokenRevocationService;

        public RevokeSessionCommandHandler(
            IRequestContext requestContext,
            IUserSessionRepository userSessionRepository,
            IUserSessionService userSessionService,
            ITokenRevocationService tokenRevocationService)
        {
            _requestContext = requestContext;
            _userSessionRepository = userSessionRepository;
            _userSessionService = userSessionService;
            _tokenRevocationService = tokenRevocationService;
        }

        public async Task<SuccessDetails<RevokeSessionResponseDto>> Handle(
            RevokeSessionCommandRequest request,
            CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            var session = await _userSessionRepository.GetByIdAsync(
                request.SessionId,
                cancellationToken);

            if (session is null || session.UserId != _requestContext.UserId.Value)
                throw new NotFoundException("Oturum bulunamadı.");

            var currentFamilyId = request.CurrentSessionFamilyId;

            var isCurrentSession = currentFamilyId.HasValue &&
                                   currentFamilyId.Value == session.FamilyId;

            await _tokenRevocationService.RevokeByFamilyIdAsync(
                session.FamilyId,
                "Kullanıcı oturumu sonlandırdı.",
                _requestContext.IpAddress,
                cancellationToken);

            await _userSessionService.RevokeByFamilyIdAsync(
                session.FamilyId,
                _requestContext.IpAddress,
                "Kullanıcı oturumu sonlandırdı.",
                cancellationToken);

            var dto = new RevokeSessionResponseDto
            {
                IsCurrentSession = isCurrentSession
            };

            return ResultResponse.Success(dto, Response.Common.OperationSuccess);
        }
    }
}