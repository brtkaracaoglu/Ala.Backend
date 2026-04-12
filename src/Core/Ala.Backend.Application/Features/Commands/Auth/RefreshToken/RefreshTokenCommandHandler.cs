using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, SuccessDetails<RefreshTokenCommandResult>>
    {
        private readonly ITokenLifeCycleService _tokenLifeCycleService;
        private readonly IRequestContext _requestContext;
        private readonly IUserSessionService _userSessionService;

        public RefreshTokenCommandHandler(
            ITokenLifeCycleService tokenLifeCycleService,
            IRequestContext requestContext,
            IUserSessionService userSessionService)
        {
            _tokenLifeCycleService = tokenLifeCycleService;
            _requestContext = requestContext;
            _userSessionService = userSessionService;
        }

        public async Task<SuccessDetails<RefreshTokenCommandResult>> Handle(
            RefreshTokenCommandRequest request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                throw new UnauthorizedException("Refresh token bulunamadı.");

            var tokenPair = await _tokenLifeCycleService.RotateRefreshTokenAsync(
                request.RefreshToken,
                _requestContext,
                cancellationToken: cancellationToken);

            await _userSessionService.TouchAsync(
                tokenPair.FamilyId,
                cancellationToken);

            return ResultResponse.Success(
                new RefreshTokenCommandResult
                {
                    AccessToken = tokenPair.AccessToken,
                    AccessTokenExpiresAtUtc = tokenPair.AccessTokenExpiresAtUtc,
                    RefreshToken = tokenPair.RefreshToken,
                    RefreshTokenExpiresAtUtc = tokenPair.RefreshTokenExpiresAtUtc
                },
                Response.Common.OperationSuccess);
        }
    }
}