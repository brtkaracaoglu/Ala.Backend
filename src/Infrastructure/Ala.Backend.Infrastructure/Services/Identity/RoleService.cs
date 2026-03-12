using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ala.Backend.Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Role?> FindByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<Role?> FindByNameAsync(string name)
        {
            return await _roleManager.FindByNameAsync(name);
        }

        public async Task<IList<Claim>> GetClaimsAsync(Role role)
        {
            return await _roleManager.GetClaimsAsync(role);
        }

        public async Task<IdentityResult> CreateAsync(Role role)
        {
            var result = await _roleManager.CreateAsync(role);
            return result;
        }

        public async Task<IdentityResult> UpdateAsync(Role role)
        {
            var result = await _roleManager.UpdateAsync(role);
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(Role role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return result;
        }

        public async Task<IdentityResult> AddClaimAsync(Role role, Claim claim)
        {
            var result = await _roleManager.AddClaimAsync(role, claim);
            return result;
        }

        public async Task<IdentityResult> RemoveClaimAsync(Role role, Claim claim)
        {
            var result = await _roleManager.RemoveClaimAsync(role, claim);
            return result;
        }
    }
}
