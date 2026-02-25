using Ala.Backend.Domain.Common;

namespace Ala.Backend.Application.Abstractions.Persistence
{
    public interface IWriteRepository<TEntity, TId> where TEntity : BaseEntity<TId> where TId : notnull
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }
}