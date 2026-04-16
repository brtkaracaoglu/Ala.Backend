using Ala.Backend.Application.Common.Identity;
using Ala.Backend.Domain.Identity;
using System.Security.Claims;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity
{
    public interface IRoleService
    {
        Task<Role?> FindByIdAsync(string id);
        Task<Role?> FindByNameAsync(string name);
        Task<IList<Claim>> GetClaimsAsync(Role role);

        Task<IdentityOperationResult> CreateAsync(Role role);
        Task<IdentityOperationResult> UpdateAsync(Role role);
        Task<IdentityOperationResult> DeleteAsync(Role role);

        Task<IdentityOperationResult> AddClaimAsync(Role role, Claim claim);
        Task<IdentityOperationResult> RemoveClaimAsync(Role role, Claim claim);
    }
}