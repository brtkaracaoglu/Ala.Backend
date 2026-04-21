using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Services.Queries
{
    public class RoleQueryService : IRoleQueryService
    {
        private readonly MainDbContext _context;

        public RoleQueryService(MainDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<RoleDto>> GetPagedAsync(
            RoleListFilter request,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Roles
                .AsNoTracking()
                .AsQueryable();

            query = ApplySearch(query, request.Search);
            query = ApplySorting(query, request.SortBy, request.Desc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await GetPagedItemsAsync(query, request, cancellationToken);

            return new PagedResponse<RoleDto>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<Domain.Identity.Role> ApplySearch(
            IQueryable<Domain.Identity.Role> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();

            return query.Where(x =>
                x.Name != null &&
                x.Name.Contains(search));
        }

        private static IQueryable<Domain.Identity.Role> ApplySorting(
            IQueryable<Domain.Identity.Role> query,
            string? sortBy,
            bool desc)
        {
            return sortBy?.ToLower() switch
            {
                "name" => desc
                    ? query.OrderByDescending(x => x.Name)
                    : query.OrderBy(x => x.Name),

                _ => desc
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id)
            };
        }

        private static Task<List<RoleDto>> GetPagedItemsAsync(IQueryable<Domain.Identity.Role> query,  RoleListFilter request,
            CancellationToken cancellationToken)
        {
            return query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new RoleDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync(cancellationToken);
        }
    }
}