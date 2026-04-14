using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Repositories.Identity
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MainDbContext _context;

        public RoleRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<RoleDto>> GetPagedAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Roles
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var roles = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new RoleDto
                {
                    Id = x.Id,
                    Name = x.Name!
                })
                .ToListAsync(cancellationToken);

            return new PagedResponse<RoleDto>
            {
                Items = roles,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}