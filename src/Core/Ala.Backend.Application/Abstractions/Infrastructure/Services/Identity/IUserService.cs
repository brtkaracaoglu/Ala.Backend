using Ala.Backend.Application.Common.Identity;
using Ala.Backend.Domain.Identity;
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

        Task<IdentityOperationResult> CreateAsync(User user, string password);
        Task<IdentityOperationResult> UpdateAsync(User user);
        Task<IdentityOperationResult> DeleteAsync(User user);

        Task<IdentityOperationResult> AddToRoleAsync(User user, string roleName);
        Task<IdentityOperationResult> RemoveFromRoleAsync(User user, string roleName);

        Task<IdentityOperationResult> AddClaimAsync(User user, Claim claim);
        Task<IdentityOperationResult> RemoveClaimAsync(User user, Claim claim);

        Task<IdentityOperationResult> ResetPasswordAsync(User user, string token, string newPassword);
        Task<IdentityOperationResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
        Task<IdentityOperationResult> ConfirmEmailAsync(User user, string token);

        Task<IdentityOperationResult> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd);
        Task<IdentityOperationResult> SetLockoutEnabledAsync(User user, bool enabled);
        Task<IdentityOperationResult> ResetAccessFailedCountAsync(User user);
        Task UpdateSecurityStampAsync(User user);

        Task<string> GenerateEmailConfirmationTokenAsync(User user);
        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<PasswordSignInCheckResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure);
    }
}