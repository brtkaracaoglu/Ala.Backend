using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Domain.Common;
using Ala.Backend.Persistence.Main.Context;
using Ala.Backend.Persistence.Repositories;
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

        public IWriteRepository<TEntity, TId> WriteRepository<TEntity, TId>() where TEntity : BaseEntity<TId> where TId : notnull
        {
            var type = typeof(TEntity).Name + "Write";
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new EfWriteRepository<TEntity, TId>(_context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IWriteRepository<TEntity, TId>)_repositories[type];
        }

        public IReadRepository<TEntity, TId> ReadRepository<TEntity, TId>() where TEntity : BaseEntity<TId> where TId : notnull
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
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result;
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await _context.Database
                .BeginTransactionAsync(cancellationToken);
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

        public void Dispose()
        {
            _context.Dispose();
            _currentTransaction?.Dispose();
        }
    }
}