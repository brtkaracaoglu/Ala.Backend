using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.UserRoles;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Services.Queries
{
    public class UserRoleQueryService : IUserRoleQueryService
    {
        private readonly MainDbContext _context;

        public UserRoleQueryService(MainDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<UserRolesDto>> GetPagedAsync(UserRoleListFilter filter, CancellationToken cancellationToken = default)
        {
            var query =
                from user in _context.Users.AsNoTracking()
                join userRole in _context.UserRoles.AsNoTracking() on user.Id equals userRole.UserId
                join role in _context.Roles.AsNoTracking() on userRole.RoleId equals role.Id
                select new UserRolesDto
                {
                    UserId = user.Id,
                    UserName = user.UserName!,
                    RoleId = role.Id,
                    RoleName = role.Name!
                };

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();

                query = query.Where(x =>
                    x.UserName.Contains(search) ||
                    x.RoleName.Contains(search));
            }

            if (filter.UserId.HasValue)
                query = query.Where(x => x.UserId == filter.UserId.Value);

            if (filter.RoleId.HasValue)
                query = query.Where(x => x.RoleId == filter.RoleId.Value);

            query = filter.SortBy?.ToLower() switch
            {
                "username" => filter.Desc
                    ? query.OrderByDescending(x => x.UserName)
                    : query.OrderBy(x => x.UserName),

                "rolename" => filter.Desc
                    ? query.OrderByDescending(x => x.RoleName)
                    : query.OrderBy(x => x.RoleName),

                "userid" => filter.Desc
                    ? query.OrderByDescending(x => x.UserId)
                    : query.OrderBy(x => x.UserId),

                "roleid" => filter.Desc
                    ? query.OrderByDescending(x => x.RoleId)
                    : query.OrderBy(x => x.RoleId),

                _ => query.OrderBy(x => x.UserId).ThenBy(x => x.RoleId)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<UserRolesDto>
            {
                Items = items,
                Page = filter.Page,
                PageSize = filter.PageSize,
                TotalCount = totalCount
            };
        }
    }
}