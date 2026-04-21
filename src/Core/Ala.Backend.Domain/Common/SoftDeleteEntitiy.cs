using Ala.Backend.Domain.Abstractions;

namespace Ala.Backend.Domain.Common
{
    public abstract class SoftDeleteEntity<TId> : TrackableEntity<TId>, ISoftDelete
        where TId : notnull
    {
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAtUtc { get; private set; }
        public int? DeletedBy { get; private set; }

        public void MarkAsDeleted(int? deletedBy, DateTime? deletedAtUtc = null)
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAtUtc = deletedAtUtc ?? DateTime.UtcNow;
            DeletedBy = deletedBy;
        }

        public void Restore()
        {
            if (!IsDeleted)
                return;

            IsDeleted = false;
            DeletedAtUtc = null;
            DeletedBy = null;
        }
    }
}