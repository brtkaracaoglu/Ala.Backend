namespace Ala.Backend.Application.DTOs.Auth
{
    public class TokenPairResult
    {
        public string AccessToken { get; set; } = null!;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
        public Guid FamilyId { get; set; }
    }
}