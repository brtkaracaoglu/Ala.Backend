
namespace Ala.Backend.Domain.Abstractions
{
    public interface ITrackable
    {
        void SetCreated(int? userId, DateTime createdAtUtc);
        void SetUpdated(int? userId, DateTime updatedAtUtc);
    }
}
