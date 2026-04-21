namespace Ala.Backend.Persistence.Main.Settings
{
    public sealed class AuthCleanupSettings
    {
        public const string SectionName = "AuthCleanup";

        public int RefreshTokenRetentionDays { get; set; } = 30;
        public int UserSessionRetentionDays { get; set; } = 60;
        public int IntervalHours { get; set; } = 24;
    }
}