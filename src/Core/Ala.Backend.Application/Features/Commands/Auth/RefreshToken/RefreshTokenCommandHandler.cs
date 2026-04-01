using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, SuccessDetails<LoginResponseDto>>
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(ITokenService tokenService, IUserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        public async Task<SuccessDetails<LoginResponseDto>> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            var existingToken = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);

            if (existingToken is null)
                throw new UnauthorizedException("Geçersiz veya süresi dolmuş refresh token.");

            if (existingToken.UserId is null)
                throw new UnauthorizedException("Refresh token ile ilişkili kullanıcı bulunamadı.");

            var user = await _userService.FindByIdAsync(existingToken.UserId.Value.ToString());

            if (user is null)
                throw new UnauthorizedException("Kullanıcı bulunamadı.");

            var newRefreshToken = await _tokenService.RotateRefreshTokenAsync(user, request.RefreshToken, request.IpAddress);

            if (newRefreshToken is null)
                throw new OperationFailedException("Refresh token yenilenemedi.");

            var accessToken = await _tokenService.GenerateAccessTokenAsync(user);

            var dto = new LoginResponseDto
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                FullName = $"{user.FirstName} {user.LastName}".Trim(),
                AccessToken = accessToken.Token,
                AccessTokenExpiresAtUtc = accessToken.ExpiresAtUtc,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAtUtc = newRefreshToken.ExpiresAtUtc
            };

            return ResultResponse.Success(dto, "Token yenileme işlemi başarıyla tamamlandı.");
        }
    }
}