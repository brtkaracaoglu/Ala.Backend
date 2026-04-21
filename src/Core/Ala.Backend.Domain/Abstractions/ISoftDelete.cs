namespace Ala.Backend.Domain.Abstractions
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; }
        DateTime? DeletedAtUtc { get; }
        int? DeletedBy { get; }

        void MarkAsDeleted(int? deletedBy, DateTime? deletedAtUtc = null);
        void Restore();
    }
}