namespace Ala.Backend.Domain.Abstractions
{
    public interface ITrackable
    {
        int? CreatedBy { get; }
        DateTime CreatedAtUtc { get; }

        int? UpdatedBy { get; }
        DateTime? UpdatedAtUtc { get; }

        void SetCreated(int? userId, DateTime createdAtUtc);
        void SetUpdated(int? userId, DateTime updatedAtUtc);
    }
}