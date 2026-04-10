namespace Ala.Backend.Application.DTOs.Maintenance
{
    public sealed class AuthCleanupResult
    {
        public int DeletedRefreshTokens { get; set; }
        public int DeletedUserSessions { get; set; }
        public DateTime ExecutedAtUtc { get; set; }
    }
}