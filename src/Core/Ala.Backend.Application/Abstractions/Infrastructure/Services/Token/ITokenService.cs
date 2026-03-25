using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Domain.Identity;
using System.Security.Claims;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Token
{
    public interface ITokenService
    {
        // Access Token üretimi
        Task<AccessTokenResult> GenerateAccessTokenAsync(User user, IEnumerable<Claim>? extraClaims = null);

        // Refresh token oluşturma 
        Task<RefreshToken> CreateRefreshTokenAsync(User user, string ipAddress);

        // Refresh token doğrulama
        Task<RefreshToken> ValidateRefreshTokenAsync(string refreshToken);

        // Refresh token döndürme 
        Task<RefreshToken> RotateRefreshTokenAsync(User user, string oldToken, string ipAddress);

        // Kullanıcının tüm oturumlarını kapatma
        Task RevokeAllAsync(int userId);




        // Tekil token iptali 
        Task<bool> RevokeTokenAsync(string refreshToken, string ipAddress);

        // JWT doğrulama (Access Token)
        Task<ClaimsPrincipal> ValidateJwtTokenAsync(string token, bool validateLifetime = true);
    }
}