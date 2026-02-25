using System.Linq.Expressions;
using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Repositories
{
    public class EfReadRepository<TEntity, TId> : IReadRepository<TEntity, TId> where TEntity : BaseEntity<TId> where TId : notnull
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public EfReadRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id.Equals(id),
                    cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TResult>> selector,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(predicate)
                .Select(selector)
                .ToListAsync(cancellationToken);
        }

        public async Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<TEntity, bool>>? predicate,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than 0.");

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0.");

            IQueryable<TEntity> query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            query = query.OrderBy(x => x.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
    }
}