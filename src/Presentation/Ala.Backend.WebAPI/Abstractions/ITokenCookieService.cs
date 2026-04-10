namespace Ala.Backend.Presentation.Abstractions
{
    public interface ITokenCookieService
    {
        void SetAccessToken(string accessToken, DateTime expiresAtUtc);
        void SetRefreshToken(string refreshToken, DateTime expiresAtUtc);
        string? GetRefreshToken();
        void ClearAccessToken();
        void ClearRefreshToken();
        void ClearAll();
    }
}