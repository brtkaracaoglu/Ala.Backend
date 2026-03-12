using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity
{
    public interface IRoleService
    {
        Task<Role?> FindByIdAsync(string id);
        Task<Role?> FindByNameAsync(string name);
        Task<IList<Claim>> GetClaimsAsync(Role role);

        Task<IdentityResult> CreateAsync(Role role);
        Task<IdentityResult> UpdateAsync(Role role);
        Task<IdentityResult> DeleteAsync(Role role);

        Task<IdentityResult> AddClaimAsync(Role role, Claim claim);
        Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim);
    }
}
