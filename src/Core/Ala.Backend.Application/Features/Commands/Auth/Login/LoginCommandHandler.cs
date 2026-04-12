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

namespace Ala.Backend.Application.Features.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, SuccessDetails<LoginCommandResult>>
    {
        private readonly IUserService _userService;
        private readonly ITokenLifeCycleService _tokenLifeCycleService;
        private readonly IRequestContext _requestContext;
        private readonly IUserSessionService _userSessionService;

        public LoginCommandHandler(
            IUserService userService,
            ITokenLifeCycleService tokenLifeCycleService,
            IRequestContext requestContext,
            IUserSessionService userSessionService)
        {
            _userService = userService;
            _tokenLifeCycleService = tokenLifeCycleService;
            _requestContext = requestContext;
            _userSessionService = userSessionService;
        }

        public async Task<SuccessDetails<LoginCommandResult>> Handle(
            LoginCommandRequest request,
            CancellationToken cancellationToken)
        {
            var isEmail = request.EmailOrUsername.Contains("@");

            var user = isEmail
                ? await _userService.FindByEmailAsync(request.EmailOrUsername)
                : await _userService.FindByNameAsync(request.EmailOrUsername);

            if (user is null)
                throw new UnauthorizedException("Kullanıcı adı/e-posta veya şifre hatalı.");

            var signInResult = await _userService.CheckPasswordSignInAsync(
                user,
                request.Password,
                lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
                throw new UnauthorizedException("Hesabınız geçici olarak kilitlenmiştir.");

            if (signInResult.IsNotAllowed)
            {
                if (!user.EmailConfirmed)
                    throw new UnauthorizedException("E-posta adresiniz doğrulanmamış.");

                if (!user.IsActive)
                    throw new BusinessRuleException("Pasif kullanıcılar giriş yapamaz.");

                throw new UnauthorizedException("Hesabınız giriş için uygun durumda değil.");
            }

            if (!signInResult.Succeeded)
                throw new UnauthorizedException("Kullanıcı adı/e-posta veya şifre hatalı.");

            if (user.NeedPasswordReset)
            {
                var resetToken = await _userService.GeneratePasswordResetTokenAsync(user);
                var encodedResetToken = TokenExtensions.EncodeToken(resetToken);

                var resetDto = new LoginResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    Username = user.UserName ?? string.Empty,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Roles = new List<string>(),
                    RequiresPasswordReset = true,
                    ResetPasswordToken = encodedResetToken
                };

                return ResultResponse.Success(
                    new LoginCommandResult
                    {
                        Response = resetDto
                    },
                    "Giriş başarılı. Ancak şifre yenileme işlemi gereklidir.");
            }

            var accessToken = await _tokenLifeCycleService.GenerateAccessTokenAsync(
                user,
                cancellationToken: cancellationToken);

            var refreshToken = await _tokenLifeCycleService.CreateRefreshTokenAsync(
                user,
                _requestContext,
                accessToken.JwtId,
                cancellationToken: cancellationToken);

            await _userSessionService.CreateAsync(
                user.Id,
                refreshToken.FamilyId,
                _requestContext,
                cancellationToken);

            var roles = await _userService.GetRolesAsync(user);

            var dto = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Roles = roles.ToList(),
                RequiresPasswordReset = false,
                ResetPasswordToken = null
            };

            return ResultResponse.Success(
                new LoginCommandResult
                {
                    Response = dto,
                    AccessToken = accessToken.Token,
                    AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                    RefreshToken = refreshToken.Token,
                    RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc
                },
                Response.Common.OperationSuccess);
        }
    }
}