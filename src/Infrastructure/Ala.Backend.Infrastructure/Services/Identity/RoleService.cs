using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Identity;
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

        public async Task<IdentityOperationResult> CreateAsync(Role role)
        {
            var result = await _roleManager.CreateAsync(role);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> UpdateAsync(Role role)
        {
            var result = await _roleManager.UpdateAsync(role);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> DeleteAsync(Role role)
        {
            var result = await _roleManager.DeleteAsync(role);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> AddClaimAsync(Role role, Claim claim)
        {
            var result = await _roleManager.AddClaimAsync(role, claim);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> RemoveClaimAsync(Role role, Claim claim)
        {
            var result = await _roleManager.RemoveClaimAsync(role, claim);
            return MapIdentityResult(result);
        }

        private static IdentityOperationResult MapIdentityResult(IdentityResult result)
        {
            if (result.Succeeded)
                return IdentityOperationResult.Success();

            var errors = result.Errors.Select(error => new IdentityOperationError
            {
                Code = error.Code,
                Description = error.Description,
                PropertyName = ResolvePropertyName(error.Code)
            });

            return IdentityOperationResult.Failed(errors);
        }

        private static string? ResolvePropertyName(string code)
        {
            return code switch
            {
                "DuplicateRoleName" => "Name",
                "InvalidRoleName" => "Name",
                _ => null
            };
        }
    }
}