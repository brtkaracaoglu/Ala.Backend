using System.Text.Json.Serialization;
using Ala.Backend.Application.Contracts.Auth;

namespace Ala.Backend.Application.DTOs.Auth
{
    public sealed class LogoutCommandResult : ITokenCookieMutation
    {
        [JsonIgnore]
        public string? AccessToken => null;

        [JsonIgnore]
        public DateTime? AccessTokenExpiresAtUtc => null;

        [JsonIgnore]
        public string? RefreshToken => null;

        [JsonIgnore]
        public DateTime? RefreshTokenExpiresAtUtc => null;

        [JsonIgnore]
        public bool ClearAccessTokenCookie { get; set; }

        [JsonIgnore]
        public bool ClearRefreshTokenCookie { get; set; }
    }
}