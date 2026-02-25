using Ala.Backend.Domain.Abstractions;

namespace Ala.Backend.Domain.Common
{
    public abstract class SoftDeleteEntity<TId> : TrackableEntity<TId>, ISoftDelete where TId : notnull
    {
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        public string? DeletedBy { get; private set; }

        public void MarkAsDeleted(string deletedBy)
        {
            if (IsDeleted)
                return;

            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = deletedBy;

        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
        }
    }
}
