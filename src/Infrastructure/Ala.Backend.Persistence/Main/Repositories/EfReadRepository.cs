using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Domain.Common;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Ala.Backend.Persistence.Repositories
{
    public class EfReadRepository<TEntity, TId> : IReadRepository<TEntity, TId> where TEntity : BaseEntity<TId> where TId : notnull
    {
        protected readonly MainDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfReadRepository(MainDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AsNoTracking().Where(predicate).Select(selector).ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
    }
}