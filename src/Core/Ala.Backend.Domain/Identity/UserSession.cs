using Ala.Backend.Domain.Common;

namespace Ala.Backend.Domain.Identity
{
    public class UserSession : BaseEntity<long>
    {
        public int UserId { get; set; }

        // Refresh token ailesi ile ilişki
        public Guid FamilyId { get; set; }

        public string CreatedByIp { get; set; } = "N/A";
        public string? CreatedByUserAgent { get; set; }

        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime LastActivityOnUtc { get; private set; } = DateTime.UtcNow;

        public DateTime? RevokedAtUtc { get; private set; }
        public string? RevokedByIp { get; private set; }
        public string? ReasonRevoked { get; private set; }

        public bool IsRevoked => RevokedAtUtc.HasValue;
        public bool IsActive => !IsRevoked;

        public User User { get; set; } = null!;

        public void Touch()
        {
            if (IsRevoked)
                return;

            LastActivityOnUtc = DateTime.UtcNow;
        }

        public void Revoke(string revokedByIp, string? reason = null)
        {
            if (IsRevoked)
                return;

            if (string.IsNullOrWhiteSpace(revokedByIp))
                throw new InvalidOperationException("RevokedByIp boş olamaz.");

            RevokedAtUtc = DateTime.UtcNow;
            RevokedByIp = revokedByIp;
            ReasonRevoked = reason;
        }
    }
}