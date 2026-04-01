using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommandRequest, SuccessDetails>
    {
        private readonly ITokenService _tokenService;
        private readonly IRequestContext _requestContext;

        public LogoutCommandHandler(ITokenService tokenService, IRequestContext requestContext)
        {
            _tokenService = tokenService;
            _requestContext = requestContext;
        }

        public async Task<SuccessDetails> Handle(LogoutCommandRequest request, CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            await _tokenService.RevokeAllAsync(_requestContext.UserId.Value);

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}