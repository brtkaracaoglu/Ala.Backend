using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ala.Backend.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User?> FindByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<User?> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IList<string>> GetRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<IList<Claim>> GetClaimsAsync(User user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<bool> IsLockedOutAsync(User user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public async Task<IdentityResult> UpdateAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            return result;
        }

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result;
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(User user, string roleName)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return result;
        }

        public async Task<IdentityResult> AddClaimAsync(User user, Claim claim)
        {
            var result = await _userManager.AddClaimAsync(user, claim);
            return result;
        }

        public async Task<IdentityResult> RemoveClaimAsync(User user, Claim claim)
        {
            var result = await _userManager.RemoveClaimAsync(user, claim);
            return result;
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return result;
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return result;
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return result;
        }

        public async Task<IdentityResult> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd)
        {
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            return result;
        }

        public async Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled)
        {
            var result = await _userManager.SetLockoutEnabledAsync(user, enabled);
            return result;
        }

        public async Task<IdentityResult> ResetAccessFailedCountAsync(User user)
        {
            var result = await _userManager.ResetAccessFailedCountAsync(user);
            return result;
        }

        public async Task UpdateSecurityStampAsync(User user)
        {
            await _userManager.UpdateSecurityStampAsync(user);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}

