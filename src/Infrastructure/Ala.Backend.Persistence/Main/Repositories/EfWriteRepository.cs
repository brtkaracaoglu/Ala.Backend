using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Repositories
{
    public class EfWriteRepository<TEntity, TId> : IWriteRepository<TEntity, TId> where TEntity : BaseEntity<TId>
        where TId : notnull
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfWriteRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));

            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }
        public void Update(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.UpdateRange(entities);
        }

        public void Remove(TEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            if (entities is null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }
    }
}
