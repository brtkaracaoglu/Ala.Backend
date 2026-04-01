using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.Extensions;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommandRequest, SuccessDetails<LoginResponseDto>>
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IRequestContext _requestContext;

        public LoginCommandHandler(IUserService userService, ITokenService tokenService, IRequestContext requestContext)
        {
            _userService = userService;
            _tokenService = tokenService;
            _requestContext = requestContext;
        }

        public async Task<SuccessDetails<LoginResponseDto>> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
        {
            var isEmail = request.EmailOrUsername.Contains("@");

            var user = isEmail
                ? await _userService.FindByEmailAsync(request.EmailOrUsername)
                : await _userService.FindByNameAsync(request.EmailOrUsername);

            if (user is null)
                throw new UnauthorizedException("Kullanıcı adı/e-posta veya şifre hatalı.");

            var signInResult = await _userService.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
                throw new UnauthorizedException("Hesabınız geçici olarak kilitlenmiştir.");

            if (!signInResult.Succeeded)
                throw new UnauthorizedException("Kullanıcı adı/e-posta veya şifre hatalı.");

            if (!user.IsActive)
                throw new BusinessRuleException("Pasif kullanıcılar giriş yapamaz.");

            if (!user.EmailConfirmed)
                throw new UnauthorizedException("E-posta adresiniz doğrulanmamış.");

            if (user.NeedPasswordReset)
            {
                var resetToken = await _userService.GeneratePasswordResetTokenAsync(user);
                var encodedResetToken = TokenExtensions.EncodeToken(resetToken);

                var dtoo = new LoginResponseDto
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    Username = user.UserName ?? string.Empty,
                    FullName = $"{user.FirstName} {user.LastName}".Trim(),
                    Roles = new List<string>(),
                    RequiresPasswordReset = true,
                    ResetPasswordToken = encodedResetToken
                };
                return ResultResponse.Success(dtoo, "Giriş başarılı. Ancak şifre yenileme işlemi gereklidir.");
            }

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user, _requestContext.IpAddress);
            var roles = await _userService.GetRolesAsync(user);

            var dto = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                Roles = roles.ToList(),
                AccessToken = accessToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
                RequiresPasswordReset = false
            };

            return ResultResponse.Success(dto, "Giriş işlemi başarıyla tamamlandı.");
        }
    }
}