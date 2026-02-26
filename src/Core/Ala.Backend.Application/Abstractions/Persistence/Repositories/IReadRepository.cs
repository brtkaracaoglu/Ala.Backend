using System.Linq.Expressions;
using Ala.Backend.Domain.Common;

namespace Ala.Backend.Application.Abstractions.Persistence
{
    public interface IReadRepository<TEntity, TId> where TEntity : BaseEntity<TId> where TId : notnull
    {

        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, bool>> predicate,Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default);

        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default);

        IQueryable<TEntity> Query();
    }
}