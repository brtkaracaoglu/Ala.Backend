using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Domain.Identity;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ala.Backend.Infrastructure.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        public TokenService(IOptions<JwtSettings> jwtOptions, UserManager<User> userManager, RoleManager<Role> roleManager, IUnitOfWork unitOfWork)
        {
            _jwtSettings = jwtOptions.Value; 
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<AccessTokenResult> GenerateAccessTokenAsync(User user, IEnumerable<Claim>? extraClaims = null)
        {
            var claims = await GetAllUserClaimsAsync(user, extraClaims);

            var now = DateTime.UtcNow;
            var expiresAt = now.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes);

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                NotBefore = now,
                Expires = expiresAt,
                SigningCredentials = GetSigningCredentials(),
                EncryptingCredentials = GetEncryptingCredentials()
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);

            return new AccessTokenResult
            {
                Token = handler.WriteToken(token),
                ExpiresAtUtc = expiresAt
            };
        }

        private async Task<List<Claim>> GetAllUserClaimsAsync(User user, IEnumerable<Claim>? extraClaims)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var userClaimsTask = _userManager.GetClaimsAsync(user);
            var userRolesTask = _userManager.GetRolesAsync(user);

            await Task.WhenAll(userClaimsTask, userRolesTask);

            claims.AddRange(userClaimsTask.Result);

            foreach (var roleName in userRolesTask.Result)
            {
                claims.Add(new Claim(ClaimTypes.Role, roleName));
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    claims.AddRange(roleClaims);
                }
            }

            if (extraClaims != null) claims.AddRange(extraClaims);

            return claims;
        }

        public async Task<RefreshToken> CreateRefreshTokenAsync(User user, string ipAddress)
        {
            var now = DateTime.UtcNow;

            var refresh = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiresAtUtc = now.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                CreatedByIp = ipAddress
            };

            await _unitOfWork.WriteRepository<RefreshToken, int>().AddAsync(refresh);
            await _unitOfWork.SaveChangesAsync();

            return refresh;
        }

        public async Task<RefreshToken> ValidateRefreshTokenAsync(string refreshToken)
        {
            var token = await _unitOfWork.ReadRepository<RefreshToken, int>().GetAsync(x => x.Token == refreshToken);

            if (token == null)
            {
                throw new UnauthorizedException("Geçersiz yenileme token'ı.");
            }

            if (!token.IsActive)
            {
                throw new UnauthorizedException("Yenileme token'ının süresi dolmuş.");
            }

            return token;
        }

        public async Task<RefreshToken> RotateRefreshTokenAsync(User user, string oldToken, string ipAddress)
        {
            var existing = await _unitOfWork.ReadRepository<RefreshToken, int>().GetAsync(x => x.Token == oldToken);

            if (existing == null)
            {
                throw new UnauthorizedException("Yenileme token'ı bulunamadı.");
            }

            if (existing.IsRevoked)
            {
                await RevokeAllAsync(user.Id);
                throw new UnauthorizedException("Güvenlik uyarısı: Bu token daha önce iptal edilmiş.");
            }

            if (!existing.IsActive)
            {
                throw new UnauthorizedException("Yenileme token'ının süresi dolmuş.");
            }
            var now = DateTime.UtcNow;
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                ExpiresAtUtc = now.AddDays(_jwtSettings.RefreshTokenExpiryDays),
                CreatedByIp = ipAddress,
            };

            existing.Revoke(newRefreshToken.Token, ipAddress, reason: "Rotated");
            existing.MarkAsUsed();

            _unitOfWork.WriteRepository<RefreshToken, int>().Update(existing);
            await _unitOfWork.WriteRepository<RefreshToken, int>().AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return newRefreshToken;
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress)
        {
            var token = await _unitOfWork.ReadRepository<RefreshToken, int>().GetAsync(x => x.Token == refreshToken);
            if (token is null) return false;

            token.Revoke(null, ipAddress, "Manually Revoked");

            _unitOfWork.WriteRepository<RefreshToken, int>().Update(token);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task RevokeAllAsync(int userId)
        {
            var tokens = await _unitOfWork.ReadRepository<RefreshToken, int>().GetListAsync(x => x.UserId == userId && x.RevokedAtUtc == null); if (!tokens.Any()) return;

            foreach (var t in tokens)
                t.Revoke(null, "System", "Revoke All Requested");

            _unitOfWork.WriteRepository<RefreshToken, int>().UpdateRange(tokens);
            await _unitOfWork.SaveChangesAsync();
        }
        public Task<ClaimsPrincipal> ValidateJwtTokenAsync(string token, bool validateLifetime = true)
        {
            var validation = new TokenValidationParameters
            {
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey)),
                TokenDecryptionKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.EncryptionKey)),
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, validation, out SecurityToken securityToken);

                if (securityToken is JwtSecurityToken jwt)
                {
                    if (jwt.Header.Alg != SecurityAlgorithms.HmacSha256 ||
                        jwt.Header.Enc != SecurityAlgorithms.Aes256CbcHmacSha512)
                    {
                        throw new SecurityTokenException("Invalid token algorithm");
                    }
                }


                return Task.FromResult(principal);
            }
            catch (Exception)
            {
                throw new UnauthorizedException("Geçersiz veya süresi dolmuş anahtar.");
            }
        }

        private SigningCredentials GetSigningCredentials()
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningKey));
            return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        }

        private EncryptingCredentials GetEncryptingCredentials()
        {
            var encryptionKey = new SymmetricSecurityKey(Convert.FromBase64String(_jwtSettings.EncryptionKey));
            return new EncryptingCredentials(
                encryptionKey,
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes256CbcHmacSha512
            );
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64))
               .Replace("+", "-").Replace("/", "_").Replace("=", "");
        }

    }
}
