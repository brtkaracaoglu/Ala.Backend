using Ala.Backend.Presentation.Abstractions;
using Ala.Backend.WebAPI.Authentication;
using Microsoft.Extensions.Options;

namespace Ala.Backend.WebAPI.Services
{
    public sealed class TokenCookieService : ITokenCookieService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtCookieSettings _jwtCookieSettings;

        public TokenCookieService(
            IHttpContextAccessor httpContextAccessor,
            IOptions<JwtCookieSettings> jwtCookieOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _jwtCookieSettings = jwtCookieOptions.Value;
        }

        public void SetAccessToken(string accessToken, DateTime expiresAtUtc)
        {
            var context = GetHttpContext();

            context.Response.Cookies.Append(
                JwtCookieNames.AccessToken,
                accessToken,
                BuildCookieOptions(expiresAtUtc, _jwtCookieSettings.AccessTokenPath));
        }

        public void SetRefreshToken(string refreshToken, DateTime expiresAtUtc)
        {
            var context = GetHttpContext();

            context.Response.Cookies.Append(
                JwtCookieNames.RefreshToken,
                refreshToken,
                BuildCookieOptions(expiresAtUtc, _jwtCookieSettings.RefreshTokenPath));
        }

        public string? GetRefreshToken()
        {
            var context = GetHttpContext();

            return context.Request.Cookies.TryGetValue(JwtCookieNames.RefreshToken, out var refreshToken)
                ? refreshToken
                : null;
        }

        public void ClearAccessToken()
        {
            var context = GetHttpContext();

            context.Response.Cookies.Delete(
                JwtCookieNames.AccessToken,
                BuildDeleteCookieOptions(_jwtCookieSettings.AccessTokenPath));
        }

        public void ClearRefreshToken()
        {
            var context = GetHttpContext();

            context.Response.Cookies.Delete(
                JwtCookieNames.RefreshToken,
                BuildDeleteCookieOptions(_jwtCookieSettings.RefreshTokenPath));
        }

        public void ClearAll()
        {
            ClearAccessToken();
            ClearRefreshToken();
        }

        private HttpContext GetHttpContext()
        {
            return _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("Active HttpContext bulunamadı.");
        }

        private CookieOptions BuildCookieOptions(DateTime expiresAtUtc, string path)
        {
            return new CookieOptions
            {
                HttpOnly = _jwtCookieSettings.HttpOnly,
                Secure = _jwtCookieSettings.Secure,
                SameSite = _jwtCookieSettings.SameSite,
                Expires = new DateTimeOffset(expiresAtUtc),
                Path = path,
                IsEssential = _jwtCookieSettings.IsEssential
            };
        }

        private CookieOptions BuildDeleteCookieOptions(string path)
        {
            return new CookieOptions
            {
                HttpOnly = _jwtCookieSettings.HttpOnly,
                Secure = _jwtCookieSettings.Secure,
                SameSite = _jwtCookieSettings.SameSite,
                Path = path,
                IsEssential = _jwtCookieSettings.IsEssential
            };
        }
    }
}