using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Repositories.Identity
{
    public class UserRepository : IUserRepository
    {
        private readonly MainDbContext _context;

        public UserRepository(MainDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<UserDto>> GetPagedUsersWithRolesAsync(UserListFilter filter, CancellationToken cancellationToken = default)
        {
            IQueryable<Domain.Identity.User> query = _context.Users
                .AsNoTracking();

            query = ApplyFilters(query, filter);
            query = ApplySorting(query, filter.SortBy, filter.SortDirection);

            var totalCount = await query.CountAsync(cancellationToken);

            var users = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    UserName = x.UserName!,
                    Email = x.Email!,
                    FirstName = x.FirstName!,
                    LastName = x.LastName!,
                    IsActive = x.IsActive,
                    Roles = x.UserRoles
                        .Select(ur => ur.Role.Name!)
                        .ToList()
                })
                .ToListAsync(cancellationToken);

            return new PagedResponse<UserDto>
            {
                Items = users,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<Domain.Identity.User> ApplyFilters(IQueryable<Domain.Identity.User> query, UserListFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    x.UserName!.Contains(search) ||
                    x.Email!.Contains(search) ||
                    x.FirstName!.Contains(search) ||
                    x.LastName!.Contains(search));
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(x => x.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                var role = filter.Role.Trim();

                query = query.Where(x =>
                    x.UserRoles.Any(ur => ur.Role.Name == role));
            }

            return query;
        }

        private static IQueryable<Domain.Identity.User> ApplySorting(IQueryable<Domain.Identity.User> query, string? sortBy, string? sortDirection)
        {
            var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

            return sortBy?.ToLowerInvariant() switch
            {
                "username" => isDesc ? query.OrderByDescending(x => x.UserName) : query.OrderBy(x => x.UserName),
                "email" => isDesc ? query.OrderByDescending(x => x.Email) : query.OrderBy(x => x.Email),
                "firstname" => isDesc ? query.OrderByDescending(x => x.FirstName) : query.OrderBy(x => x.FirstName),
                "lastname" => isDesc ? query.OrderByDescending(x => x.LastName) : query.OrderBy(x => x.LastName),
                _ => isDesc ? query.OrderByDescending(x => x.Id) : query.OrderBy(x => x.Id)
            };
        }
    }
}