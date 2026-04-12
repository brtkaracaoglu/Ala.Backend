using System.Text.Json.Serialization;
using Ala.Backend.Application.Contracts.Auth;

namespace Ala.Backend.Application.DTOs.Auth
{
    public sealed class RefreshTokenCommandResult : ITokenCookieMutation
    {
        [JsonIgnore]
        public string? AccessToken { get; set; }

        [JsonIgnore]
        public DateTime? AccessTokenExpiresAtUtc { get; set; }

        [JsonIgnore]
        public string? RefreshToken { get; set; }

        [JsonIgnore]
        public DateTime? RefreshTokenExpiresAtUtc { get; set; }

        [JsonIgnore]
        public bool ClearAccessTokenCookie => false;

        [JsonIgnore]
        public bool ClearRefreshTokenCookie => false;
    }
}