namespace Ala.Backend.Application.DTOs.Auth
{
    public class UserSessionDto
    {
        public long Id { get; set; }
        public Guid FamilyId { get; set; }
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;

        public string Browser { get; set; } = "Unknown";
        public string Platform { get; set; } = "Unknown";
        public string Device { get; set; } = "Unknown";
        public string DisplayName { get; set; } = "Unknown device";

        public DateTime CreatedOnUtc { get; set; }
        public DateTime LastActivityOnUtc { get; set; }

        public bool IsCurrent { get; set; }
        public bool IsActive { get; set; }

        public DateTime? RevokedAtUtc { get; set; }
        public string? ReasonRevoked { get; set; }
    }
}
