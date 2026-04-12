namespace Ala.Backend.Application.DTOs.Auth
{
    public sealed class RefreshTokenResult
    {
        public string Token { get; init; } = null!;
        public DateTime ExpiresAtUtc { get; init; }
        public Guid FamilyId { get; set; }
    }
}