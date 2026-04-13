using Ala.Backend.Application.Abstractions.Persistence.Service.Permission;
using Ala.Backend.Domain.Identity;
using Ala.Backend.Persistence.Main.Context;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Services.Permissions
{
    public sealed class PermissionSeeder : IPermissionSeeder
    {
        private readonly MainDbContext _context;

        public PermissionSeeder(MainDbContext context)
        {
            _context = context;
        }

        public async Task SyncPermissionsAsync(IEnumerable<string> permissionCodes)
        {
            var normalizedPermissions = permissionCodes
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .Select(p => p.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var existing = (await _context.Permissions
                .AsNoTracking()
                .Select(x => x.Code)
                .ToListAsync())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newPermissions = normalizedPermissions
                .Where(p => !existing.Contains(p))
                .Select(p => new Permission
                {
                    Code = p,
                    Description = $"Permission for {p}"
                })
                .ToList();

            if (newPermissions.Count == 0)
                return;

            await _context.Permissions.AddRangeAsync(newPermissions);
            await _context.SaveChangesAsync();
        }
    }
}