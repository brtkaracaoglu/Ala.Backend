using Ala.Backend.Domain.Abstractions;

namespace Ala.Backend.Domain.Common
{
    public abstract class TrackableEntity<TId> : BaseEntity<TId>, ITrackable where TId : notnull
    {
        public int? CreatedBy { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        public int? UpdatedBy { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        public void SetCreated(int? userId, DateTime createdAtUtc)
        {
            if (CreatedAtUtc != default)
                throw new InvalidOperationException("Created information already set.");

            CreatedBy = userId;
            CreatedAtUtc = createdAtUtc;
        }

        public void SetUpdated(int? userId, DateTime updatedAtUtc)
        {
            UpdatedBy = userId;
            UpdatedAtUtc = updatedAtUtc;
        }
    }
}