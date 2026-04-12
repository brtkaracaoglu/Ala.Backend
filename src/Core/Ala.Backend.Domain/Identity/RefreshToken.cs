using Ala.Backend.Domain.Common;

namespace Ala.Backend.Domain.Identity
{
    public class RefreshToken : BaseEntity<long>
    {
        public int UserId { get; set; }

        // Raw token değil, hash saklanır
        public string TokenHash { get; set; } = null!;

        // Aynı oturum ailesini takip etmek için
        public Guid FamilyId { get; set; }

        // Rotation zinciri
        public long? ParentRefreshTokenId { get; set; }
        public string? ReplacedByTokenHash { get; private set; }

        // Access token korelasyonu
        public string JwtId { get; set; } = null!;

        public DateTime ExpiresAtUtc { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        // Audit
        public string CreatedByIp { get; set; } = "N/A";
        public string? CreatedByUserAgent { get; set; }

        public string? RevokedByIp { get; private set; }
        public string? ReasonRevoked { get; private set; }
        public DateTime? RevokedAtUtc { get; private set; }

        public bool IsUsed { get; private set; }
        public DateTime? UsedAtUtc { get; private set; }

        public bool IsRevoked => RevokedAtUtc.HasValue;
        public bool IsExpired => ExpiresAtUtc <= DateTime.UtcNow;
        public bool IsActive => !IsUsed && !IsRevoked && !IsExpired;

        public User User { get; set; } = null!;

        public void MarkAsUsed()
        {
            if (IsUsed)
                return;

            IsUsed = true;
            UsedAtUtc = DateTime.UtcNow;
        }

        public void Revoke(string? replacedByTokenHash, string ipAddress, string? reason = null)
        {
            if (IsRevoked)
                return;

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new InvalidOperationException("RevokedByIp boş olamaz.");

            RevokedAtUtc = DateTime.UtcNow;
            ReplacedByTokenHash = replacedByTokenHash;
            RevokedByIp = ipAddress;
            ReasonRevoked = reason;
        }
    }
}