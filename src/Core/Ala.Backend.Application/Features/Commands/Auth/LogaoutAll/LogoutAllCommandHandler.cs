using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.LogoutAll
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommandRequest, SuccessDetails<LogoutCommandResult>>
    {
        private readonly IRequestContext _requestContext;
        private readonly IUserSessionService _userSessionService;
        private readonly ITokenRevocationService _tokenRevocationService;

        public LogoutAllCommandHandler(
            IRequestContext requestContext,
            IUserSessionService userSessionService,
            ITokenRevocationService tokenRevocationService)
        {
            _requestContext = requestContext;
            _userSessionService = userSessionService;
            _tokenRevocationService = tokenRevocationService;
        }

        public async Task<SuccessDetails<LogoutCommandResult>> Handle(
            LogoutAllCommandRequest request,
            CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            await _tokenRevocationService.RevokeAllAsync(
                _requestContext.UserId.Value,
                reason: "Kullanıcı tüm oturumlardan çıkış yapsın.",
                revokedByIp: _requestContext.IpAddress,
                cancellationToken: cancellationToken);

            await _userSessionService.RevokeAllAsync(
                _requestContext.UserId.Value,
                _requestContext.IpAddress,
                "Kullanıcı tüm oturumlardan çıkış yaptı.",
                cancellationToken);

            return ResultResponse.Success(
                new LogoutCommandResult
                {
                    ClearAccessTokenCookie = true,
                    ClearRefreshTokenCookie = true
                },
                Response.Common.OperationSuccess);
        }
    }
}