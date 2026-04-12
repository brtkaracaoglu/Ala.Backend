using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Domain.Identity;
using System.Security.Claims;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Token
{
    public interface ITokenLifeCycleService
    {
        Task<AccessTokenResult> GenerateAccessTokenAsync(
            User user,
            IEnumerable<Claim>? extraClaims = null,
            CancellationToken cancellationToken = default);

        Task<RefreshTokenResult> CreateRefreshTokenAsync(
            User user,
            IRequestContext requestContext,
            string jwtId,
            Guid? familyId = null,
            long? parentRefreshTokenId = null,
            CancellationToken cancellationToken = default);

        Task<RefreshToken> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default);

        Task<TokenPairResult> RotateRefreshTokenAsync(
            string oldRefreshToken,
            IRequestContext requestContext,
            IEnumerable<Claim>? extraClaims = null,
            CancellationToken cancellationToken = default);
    }
}