
using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity
{
    public interface IUserService
    {
        Task<User?> FindByIdAsync(string id);
        Task<User?> FindByEmailAsync(string email);
        Task<User?> FindByNameAsync(string userName);
        Task<IList<string>> GetRolesAsync(User user);
        Task<IList<Claim>> GetClaimsAsync(User user);
        Task<IList<User>> GetUsersInRoleAsync(string roleName);
        Task<bool> IsInRoleAsync(User user, string roleName);
        Task<bool> IsLockedOutAsync(User user);

        Task<IdentityResult> CreateAsync(User user, string password);
        Task<IdentityResult> UpdateAsync(User user);
        Task<IdentityResult> DeleteAsync(User user);

        Task<IdentityResult> AddToRoleAsync(User user, string roleName);
        Task<IdentityResult> RemoveFromRoleAsync(User user, string roleName);

        Task<IdentityResult> AddClaimAsync(User user, Claim claim);
        Task<IdentityResult> RemoveClaimAsync(User user, Claim claim);

        Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);
        Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<IdentityResult> ConfirmEmailAsync(User user, string token);

        Task<IdentityResult> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd);
        Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled);
        Task<IdentityResult> ResetAccessFailedCountAsync(User user);
        Task UpdateSecurityStampAsync(User user);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);
    }
}
