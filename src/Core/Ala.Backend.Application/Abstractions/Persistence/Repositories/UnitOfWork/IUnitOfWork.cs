using Ala.Backend.Application.Abstractions.Persistence.Repositories.Enitties;
using Ala.Backend.Domain.Common;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IWriteRepository<TEntity, TId> WriteRepository<TEntity, TId>()
            where TEntity : BaseEntity<TId>
            where TId : notnull;

        IReadRepository<TEntity, TId> ReadRepository<TEntity, TId>()
            where TEntity : BaseEntity<TId>
            where TId : notnull;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default);

        Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken = default);
    }
}