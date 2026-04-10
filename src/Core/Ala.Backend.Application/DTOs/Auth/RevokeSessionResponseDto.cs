using System.Text.Json.Serialization;
using Ala.Backend.Application.Contracts.Auth;

namespace Ala.Backend.Application.DTOs.Auth
{
    public sealed class RevokeSessionResponseDto : ITokenCookieMutation
    {
        public bool IsCurrentSession { get; set; }

        [JsonIgnore]
        public string? AccessToken => null;

        [JsonIgnore]
        public DateTime? AccessTokenExpiresAtUtc => null;

        [JsonIgnore]
        public string? RefreshToken => null;

        [JsonIgnore]
        public DateTime? RefreshTokenExpiresAtUtc => null;

        [JsonIgnore]
        public bool ClearAccessTokenCookie => IsCurrentSession;

        [JsonIgnore]
        public bool ClearRefreshTokenCookie => IsCurrentSession;
    }
}