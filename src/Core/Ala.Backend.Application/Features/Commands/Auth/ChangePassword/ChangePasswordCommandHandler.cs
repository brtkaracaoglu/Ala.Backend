using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ChangePassword
{
    public class ChangePasswordCommandHandler
        : IRequestHandler<ChangePasswordCommandRequest, SuccessDetails<LogoutCommandResult>>
    {
        private readonly IUserService _userService;
        private readonly IRequestContext _requestContext;
        private readonly IUserSessionService _userSessionService;
        private readonly ITokenRevocationService _tokenRevocationService;

        public ChangePasswordCommandHandler(
            IUserService userService,
            IRequestContext requestContext,
            IUserSessionService userSessionService,
            ITokenRevocationService tokenRevocationService)
        {
            _userService = userService;
            _requestContext = requestContext;
            _userSessionService = userSessionService;
            _tokenRevocationService = tokenRevocationService;
        }

        public async Task<SuccessDetails<LogoutCommandResult>> Handle(
            ChangePasswordCommandRequest request,
            CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            var user = await _userService.FindByIdAsync(_requestContext.UserId.Value.ToString());

            if (user is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            var check = await _userService.CheckPasswordSignInAsync(
                user,
                request.OldPassword,
                lockoutOnFailure: false);

            if (!check.Succeeded)
                throw new UnauthorizedException("Mevcut şifre hatalı.");

            var result = await _userService.ChangePasswordAsync(
                user,
                request.OldPassword,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Şifre değiştirme işlemi başarısız oldu: {errors}");
            }

            await _userService.UpdateSecurityStampAsync(user);

            await _tokenRevocationService.RevokeAllAsync(
                user.Id,
                "Şifre değiştirildiği için tüm oturumlar sonlandırıldı.",
                _requestContext.IpAddress,
                cancellationToken);

            await _userSessionService.RevokeAllAsync(
                user.Id,
                _requestContext.IpAddress,
                "Şifre değiştirildiği için tüm oturumlar sonlandırıldı.",
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