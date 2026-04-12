using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Domain.Identity;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Ala.Backend.Infrastructure.Services.Token
{
    public class TokenLifeCycleService : ITokenLifeCycleService
    {
        private const string InvalidRefreshTokenMessage = "Invalid refresh token.";
        private sealed record RequestMetadata(string IpAddress, string UserAgent);

        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRefreshTokenHasher _refreshTokenHasher;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JwtSettings _jwtSettings;

        public TokenLifeCycleService(
            IOptions<JwtSettings> jwtOptions,
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IRefreshTokenHasher refreshTokenHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtOptions.Value;
            _userManager = userManager;
            _roleManager = roleManager;
            _refreshTokenHasher = refreshTokenHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccessTokenResult> GenerateAccessTokenAsync(
            User user,
            IEnumerable<Claim>? extraClaims = null,
            CancellationToken cancellationToken = default)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var jwtId = Guid.NewGuid().ToString("N");
            var claims = await GetAllUserClaimsAsync(user, jwtId, extraClaims);

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAtUtc,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = GetSigningCredentials()
            };

            if (_jwtSettings.UseEncryption)
            {
                tokenDescriptor.EncryptingCredentials = GetEncryptingCredentials();
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            var tokenValue = handler.WriteToken(token);

            return new AccessTokenResult
            {
                Token = tokenValue,
                ExpiresAtUtc = expiresAtUtc,
                JwtId = jwtId
            };
        }

        public async Task<RefreshTokenResult> CreateRefreshTokenAsync(
            User user,
            IRequestContext requestContext,
            string jwtId,
            Guid? familyId = null,
            long? parentRefreshTokenId = null,
            CancellationToken cancellationToken = default)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (requestContext is null)
                throw new ArgumentNullException(nameof(requestContext));

            if (string.IsNullOrWhiteSpace(jwtId))
                throw new ArgumentException("JwtId boş olamaz.", nameof(jwtId));

            var now = DateTime.UtcNow;
            var rawRefreshToken = GenerateSecureRefreshToken();
            var refreshTokenHash = _refreshTokenHasher.Hash(rawRefreshToken);

            var createdByIp = string.IsNullOrWhiteSpace(requestContext.IpAddress)
                ? "N/A"
                : requestContext.IpAddress;

            var createdByUserAgent = string.IsNullOrWhiteSpace(requestContext.UserAgent)
                ? "Unknown"
                : requestContext.UserAgent;

            var entity = new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                JwtId = jwtId,
                FamilyId = familyId ?? Guid.NewGuid(),
                ParentRefreshTokenId = parentRefreshTokenId,
                ExpiresAtUtc = now.AddDays(_jwtSettings.RefreshTokenExpirationDays),
                CreatedOnUtc = now,
                CreatedByIp = createdByIp,
                CreatedByUserAgent = createdByUserAgent
            };

            await _unitOfWork.WriteRepository<RefreshToken, long>()
                .AddAsync(entity, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResult
            {
                Token = rawRefreshToken,
                ExpiresAtUtc = entity.ExpiresAtUtc,
                FamilyId = entity.FamilyId
            };
        }

        public async Task<RefreshToken> ValidateRefreshTokenAsync(
            string refreshToken,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedException(InvalidRefreshTokenMessage);

            var tokenHash = _refreshTokenHasher.Hash(refreshToken);

            var entity = await _refreshTokenRepository.GetByTokenHashWithUserAsync(
                tokenHash,
                cancellationToken);

            if (entity is null || entity.IsExpired || entity.IsRevoked || entity.IsUsed)
                throw new UnauthorizedException(InvalidRefreshTokenMessage);

            return entity;
        }

        public async Task<TokenPairResult> RotateRefreshTokenAsync(
             string oldRefreshToken,
             IRequestContext requestContext,
             IEnumerable<Claim>? extraClaims = null,
             CancellationToken cancellationToken = default)
        {
            ValidateRotateRequest(oldRefreshToken, requestContext);

            var requestMetadata = CreateRequestMetadata(requestContext);
            var oldRefreshTokenHash = _refreshTokenHasher.Hash(oldRefreshToken);

            return await _unitOfWork.ExecuteInTransactionAsync(async ct =>
            {
                var currentRefreshToken = await GetCurrentRefreshTokenOrThrowAsync(oldRefreshTokenHash, ct);

                ValidateRefreshTokenState(currentRefreshToken);

                if (currentRefreshToken.IsUsed)
                {
                    await HandleRefreshTokenReuseAsync(currentRefreshToken.UserId, requestMetadata.IpAddress, ct);
                    throw new UnauthorizedException(InvalidRefreshTokenMessage);
                }

                var newAccessToken = await GenerateAccessTokenAsync(
                    currentRefreshToken.User,
                    extraClaims,
                    ct);

                return await RotateTokenPairAsync(
                    currentRefreshToken,
                    newAccessToken,
                    requestMetadata,
                    ct);
            }, cancellationToken);
        }

        private static void ValidateRotateRequest(string oldRefreshToken, IRequestContext requestContext)
        {
            if (string.IsNullOrWhiteSpace(oldRefreshToken))
                throw new UnauthorizedException(InvalidRefreshTokenMessage);

            if (requestContext is null)
                throw new ArgumentNullException(nameof(requestContext));
        }

        private static RequestMetadata CreateRequestMetadata(IRequestContext requestContext)
        {
            return new RequestMetadata(
                string.IsNullOrWhiteSpace(requestContext.IpAddress) ? "N/A" : requestContext.IpAddress,
                string.IsNullOrWhiteSpace(requestContext.UserAgent) ? "Unknown" : requestContext.UserAgent);
        }

        private async Task<RefreshToken> GetCurrentRefreshTokenOrThrowAsync(
            string tokenHash,
            CancellationToken cancellationToken)
        {
            var currentRefreshToken = await _refreshTokenRepository.GetByTokenHashWithUserAsync(
                tokenHash,
                cancellationToken);

            if (currentRefreshToken is null || currentRefreshToken.User is null)
                throw new UnauthorizedException(InvalidRefreshTokenMessage);

            return currentRefreshToken;
        }

        private static void ValidateRefreshTokenState(RefreshToken refreshToken)
        {
            if (refreshToken.IsExpired || refreshToken.IsRevoked)
                throw new UnauthorizedException(InvalidRefreshTokenMessage);
        }

        private async Task HandleRefreshTokenReuseAsync(
            int userId,
            string requestIp,
            CancellationToken cancellationToken)
        {
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId, cancellationToken);

            foreach (var token in activeTokens)
            {
                token.Revoke(
                    replacedByTokenHash: null,
                    ipAddress: requestIp,
                    reason: "Refresh token reuse detected.");
            }

            _unitOfWork.WriteRepository<RefreshToken, long>().UpdateRange(activeTokens);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private async Task<TokenPairResult> RotateTokenPairAsync(
            RefreshToken currentRefreshToken,
            AccessTokenResult newAccessToken,
            RequestMetadata requestMetadata,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var newRawRefreshToken = GenerateSecureRefreshToken();
            var newRefreshTokenHash = _refreshTokenHasher.Hash(newRawRefreshToken);
            var newRefreshTokenExpiresAtUtc = now.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            currentRefreshToken.MarkAsUsed();
            currentRefreshToken.Revoke(
                replacedByTokenHash: newRefreshTokenHash,
                ipAddress: requestMetadata.IpAddress,
                reason: "Refresh token rotated.");

            _unitOfWork.WriteRepository<RefreshToken, long>().Update(currentRefreshToken);

            var nextRefreshToken = new RefreshToken
            {
                UserId = currentRefreshToken.UserId,
                TokenHash = newRefreshTokenHash,
                JwtId = newAccessToken.JwtId,
                FamilyId = currentRefreshToken.FamilyId,
                ParentRefreshTokenId = currentRefreshToken.Id,
                ExpiresAtUtc = newRefreshTokenExpiresAtUtc,
                CreatedOnUtc = now,
                CreatedByIp = requestMetadata.IpAddress,
                CreatedByUserAgent = requestMetadata.UserAgent
            };

            await _unitOfWork.WriteRepository<RefreshToken, long>()
                .AddAsync(nextRefreshToken, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TokenPairResult
            {
                AccessToken = newAccessToken.Token,
                AccessTokenExpiresAtUtc = newAccessToken.ExpiresAtUtc,
                RefreshToken = newRawRefreshToken,
                RefreshTokenExpiresAtUtc = newRefreshTokenExpiresAtUtc,
                FamilyId = currentRefreshToken.FamilyId
            };
        }

        private async Task<List<Claim>> GetAllUserClaimsAsync(
            User user,
            string jwtId,
            IEnumerable<Claim>? extraClaims = null)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, jwtId),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName ?? string.Empty),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty)
            };

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            foreach (var role in roles.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var roleEntity = await _roleManager.FindByNameAsync(role);
                if (roleEntity is null)
                    continue;

                var roleClaims = await _roleManager.GetClaimsAsync(roleEntity);
                claims.AddRange(roleClaims);
            }

            if (extraClaims is not null)
                claims.AddRange(extraClaims);

            return claims
                .GroupBy(x => new { x.Type, x.Value })
                .Select(x => x.First())
                .ToList();
        }

        private SigningCredentials GetSigningCredentials()
        {
            var keyBytes = DecodeBase64Key(_jwtSettings.SigningKey, nameof(_jwtSettings.SigningKey));

            if (keyBytes.Length < 32)
                throw new InvalidOperationException("SigningKey must decode to at least 32 bytes.");

            var securityKey = new SymmetricSecurityKey(keyBytes);
            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }

        private EncryptingCredentials GetEncryptingCredentials()
        {
            if (!_jwtSettings.UseEncryption)
                throw new InvalidOperationException("JWT encryption is disabled.");

            var keyBytes = DecodeBase64Key(_jwtSettings.EncryptionKey, nameof(_jwtSettings.EncryptionKey));

            if (keyBytes.Length != 32)
                throw new InvalidOperationException("EncryptionKey must decode to exactly 32 bytes.");

            var securityKey = new SymmetricSecurityKey(keyBytes);

            return new EncryptingCredentials(
                securityKey,
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes256CbcHmacSha512);
        }

        private static byte[] DecodeBase64Key(string value, string keyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{keyName} is required.");

            try
            {
                return Convert.FromBase64String(value);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException($"{keyName} must be a valid Base64 string.", ex);
            }
        }

        private static string GenerateSecureRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return WebEncoders.Base64UrlEncode(randomBytes);
        }
    }
}