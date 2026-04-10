namespace Ala.Backend.Application.Contracts.Auth
{
    public interface ITokenCookieMutation
    {
        string? AccessToken { get; }
        DateTime? AccessTokenExpiresAtUtc { get; }

        string? RefreshToken { get; }
        DateTime? RefreshTokenExpiresAtUtc { get; }

        bool ClearAccessTokenCookie { get; }
        bool ClearRefreshTokenCookie { get; }
    }
}