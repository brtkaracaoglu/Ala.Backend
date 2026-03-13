using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ala.Backend.Persistence.Main.Repositories.Identity
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleRepository(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default)
        {
            return await _roleManager.Roles
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
