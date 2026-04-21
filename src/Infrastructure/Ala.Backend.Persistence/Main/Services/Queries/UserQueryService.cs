using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using Ala.Backend.Domain.Identity;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Services.Queries
{
    public class UserQueryService : IUserQueryService
    {
        private readonly MainDbContext _context;

        public UserQueryService(MainDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<UserDto>> GetPagedAsync(
            UserListFilter request,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            query = ApplySearch(query, request.Search);
            query = ApplyIsActiveFilter(query, request.IsActive);
            query = ApplyRoleFilter(query, request.Role);
            query = ApplySorting(query, request.SortBy, request.Desc);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await GetPagedItemsAsync(query, request, cancellationToken);

            return new PagedResponse<UserDto>
            {
                Items = items,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        private static IQueryable<Domain.Identity.User> ApplySearch(
            IQueryable<Domain.Identity.User> query,
            string? search)
        {
            if (string.IsNullOrWhiteSpace(search))
                return query;

            search = search.Trim();

            return query.Where(x =>
                (x.UserName != null && x.UserName.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)) ||
                (x.FirstName != null && x.FirstName.Contains(search)) ||
                (x.LastName != null && x.LastName.Contains(search)));
        }

        private static IQueryable<Domain.Identity.User> ApplyIsActiveFilter(
            IQueryable<Domain.Identity.User> query,
            bool? isActive)
        {
            if (!isActive.HasValue)
                return query;

            return query.Where(x => x.IsActive == isActive.Value);
        }

        private static IQueryable<Domain.Identity.User> ApplyRoleFilter(
            IQueryable<Domain.Identity.User> query,
            string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return query;

            return query.Where(x =>
                x.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == role));
        }

        private static IQueryable<Domain.Identity.User> ApplySorting(
            IQueryable<Domain.Identity.User> query,
            string? sortBy,
            bool desc)
        {
            return sortBy?.ToLower() switch
            {
                "username" => desc
                    ? query.OrderByDescending(x => x.UserName)
                    : query.OrderBy(x => x.UserName),

                "email" => desc
                    ? query.OrderByDescending(x => x.Email)
                    : query.OrderBy(x => x.Email),

                "firstname" => desc
                    ? query.OrderByDescending(x => x.FirstName)
                    : query.OrderBy(x => x.FirstName),

                "lastname" => desc
                    ? query.OrderByDescending(x => x.LastName)
                    : query.OrderBy(x => x.LastName),

                _ => desc
                    ? query.OrderByDescending(x => x.Id)
                    : query.OrderBy(x => x.Id)
            };
        }

        private static Task<List<UserDto>> GetPagedItemsAsync(
            IQueryable<Domain.Identity.User> query,
            UserListFilter request,
            CancellationToken cancellationToken)
        {
            return query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    UserName = x.UserName ?? string.Empty,
                    Email = x.Email ?? string.Empty,
                    FirstName = x.FirstName ?? string.Empty,
                    LastName = x.LastName ?? string.Empty,
                    IsActive = x.IsActive,
                    Roles = x.UserRoles
                        .Where(ur => ur.Role != null && ur.Role.Name != null)
                        .Select(ur => ur.Role.Name!)
                        .ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}