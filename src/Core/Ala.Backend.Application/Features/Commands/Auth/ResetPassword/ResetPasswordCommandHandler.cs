using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.Extensions;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommandRequest, SuccessDetails<LogoutCommandResult>>
    {
        private readonly IUserService _userService;
        private readonly IUserSessionService _userSessionService;
        private readonly IRequestContext _requestContext;
        private readonly ITokenRevocationService _tokenRevocationService;

        public ResetPasswordCommandHandler(
            IUserService userService,
            IUserSessionService userSessionService,
            IRequestContext requestContext,
            ITokenRevocationService tokenRevocationService)
        {
            _userService = userService;
            _userSessionService = userSessionService;
            _requestContext = requestContext;
            _tokenRevocationService = tokenRevocationService;
        }

        public async Task<SuccessDetails<LogoutCommandResult>> Handle(
            ResetPasswordCommandRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.UserId.ToString());

            if (user is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            if (!user.IsActive)
                throw new BusinessRuleException("Pasif kullanıcılar için şifre sıfırlama işlemi yapılamaz.");

            if (!user.EmailConfirmed)
                throw new ForbiddenException("E-posta adresi doğrulanmamış kullanıcılar şifre sıfırlayamaz.");

            string decodedToken;
            try
            {
                decodedToken = TokenExtensions.DecodeToken(request.ResetToken);
            }
            catch
            {
                throw new BadRequestException("Geçersiz şifre sıfırlama token'ı.");
            }

            var result = await _userService.ResetPasswordAsync(
                user,
                decodedToken,
                request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Şifre sıfırlama işlemi başarısız oldu: {errors}");
            }

            user.NeedPasswordReset = false;

            await _userService.UpdateAsync(user);
            await _userService.UpdateSecurityStampAsync(user);

            await _tokenRevocationService.RevokeAllAsync(
                user.Id,
                "Şifre sıfırlandığı için tüm oturumlar sonlandırıldı.",
                _requestContext.IpAddress,
                cancellationToken);

            await _userSessionService.RevokeAllAsync(
                user.Id,
                _requestContext.IpAddress,
                "Şifre sıfırlandığı için tüm oturumlar sonlandırıldı.",
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