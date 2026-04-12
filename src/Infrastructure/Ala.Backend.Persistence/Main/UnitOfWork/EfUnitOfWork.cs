using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Domain.Common;
using Ala.Backend.Persistence.Main.Context;
using Ala.Backend.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ala.Backend.Persistence.UnitOfWork
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly MainDbContext _context;
        private readonly Dictionary<string, object> _repositories = new();
        private IDbContextTransaction? _currentTransaction;

        public EfUnitOfWork(MainDbContext context)
        {
            _context = context;
        }

        public IWriteRepository<TEntity, TId> WriteRepository<TEntity, TId>()
            where TEntity : BaseEntity<TId>
            where TId : notnull
        {
            var type = typeof(TEntity).Name + "Write";

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new EfWriteRepository<TEntity, TId>(_context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IWriteRepository<TEntity, TId>)_repositories[type];
        }

        public IReadRepository<TEntity, TId> ReadRepository<TEntity, TId>()
            where TEntity : BaseEntity<TId>
            where TId : notnull
        {
            var type = typeof(TEntity).Name + "Read";

            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new EfReadRepository<TEntity, TId>(_context);
                _repositories.Add(type, repositoryInstance);
            }

            return (IReadRepository<TEntity, TId>)_repositories[type];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                return;

            await _currentTransaction.CommitAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                return;

            await _currentTransaction.RollbackAsync(cancellationToken);
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }

        public async Task ExecuteInTransactionAsync(
            Func<CancellationToken, Task> operation,
            CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    _currentTransaction = transaction;

                    await operation(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
                finally
                {
                    _currentTransaction = null;
                }
            });
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    _currentTransaction = transaction;

                    var result = await operation(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
                finally
                {
                    _currentTransaction = null;
                }
            });
        }

        public void Dispose()
        {
            _currentTransaction?.Dispose();
            _context.Dispose();
        }
    }
}