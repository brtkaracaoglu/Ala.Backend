using Ala.Backend.Domain.Common;

namespace Ala.Backend.Domain.Identity
{
    public class RefreshToken : BaseEntity<int>
    {
        public int? UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }

        // Bu önemli! Token ne zaman oluştu 
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;

        //  Güvenlik & Audit Alanları  
        public string CreatedByIp { get; set; } = "N/A";
        public string? RevokedByIp { get; private set; }
        public string? ReasonRevoked { get; private set; }
        public string? ReplacedByToken { get; private set; }
        public DateTime? RevokedAtUtc { get; private set; }

        public bool IsUsed { get; private set; }

        // Revoke kontrolü
        public bool IsRevoked => RevokedAtUtc.HasValue;

        public bool IsActive =>
            !IsUsed &&
            !IsRevoked &&
            ExpiresAtUtc > DateTime.UtcNow;

        public User? User { get; set; }

        public void MarkAsUsed()
        {
            IsUsed = true;
        }

        public void Revoke(string? replacedByToken, string ipAddress, string? reason = null)
        {
            RevokedAtUtc = DateTime.UtcNow;
            ReplacedByToken = replacedByToken;
            RevokedByIp = ipAddress;
            ReasonRevoked = reason;
        }
    }
}