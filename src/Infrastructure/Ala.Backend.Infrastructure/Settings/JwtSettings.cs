namespace Ala.Backend.Infrastructure.Settings
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
        public bool UseEncryption { get; set; } 
        public string EncryptionKey { get; set; } = string.Empty; 
        public bool RequireHttpsMetadata { get; set; } = true;
        public int AccessTokenExpirationMinutes { get; set; } = 15;
        public int RefreshTokenExpirationDays { get; set; } = 7;
        public int ClockSkewSeconds { get; set; } = 60;
    }
}