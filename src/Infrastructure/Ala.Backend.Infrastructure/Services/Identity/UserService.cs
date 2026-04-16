using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Identity;
using Ala.Backend.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Ala.Backend.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
        public async Task<IList<User>> GetUsersInRoleAsync(string roleName)
        {
            return await _userManager.GetUsersInRoleAsync(roleName);
        }

        public async Task<bool> IsInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<bool> IsLockedOutAsync(User user)
        {
            return await _userManager.IsLockedOutAsync(user);
        }

        public async Task<IdentityOperationResult> CreateAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> UpdateAsync(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> DeleteAsync(User user)
        {
            var result = await _userManager.DeleteAsync(user);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> AddToRoleAsync(User user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> RemoveFromRoleAsync(User user, string roleName)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> AddClaimAsync(User user, Claim claim)
        {
            var result = await _userManager.AddClaimAsync(user, claim);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> RemoveClaimAsync(User user, Claim claim)
        {
            var result = await _userManager.RemoveClaimAsync(user, claim);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> ConfirmEmailAsync(User user, string token)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> SetLockoutEndDateAsync(User user, DateTimeOffset? lockoutEnd)
        {
            var result = await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> SetLockoutEnabledAsync(User user, bool enabled)
        {
            var result = await _userManager.SetLockoutEnabledAsync(user, enabled);
            return MapIdentityResult(result);
        }

        public async Task<IdentityOperationResult> ResetAccessFailedCountAsync(User user)
        {
            var result = await _userManager.ResetAccessFailedCountAsync(user);
            return MapIdentityResult(result);
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

        public async Task<PasswordSignInCheckResult> CheckPasswordSignInAsync(User user, string password, bool lockoutOnFailure)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
            return MapSignInResult(result);
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

        private static PasswordSignInCheckResult MapSignInResult(SignInResult result)
        {
            if (result.Succeeded)
                return PasswordSignInCheckResult.Success();

            return PasswordSignInCheckResult.Failed(
                isLockedOut: result.IsLockedOut,
                isNotAllowed: result.IsNotAllowed,
                requiresTwoFactor: result.RequiresTwoFactor);
        }

        private const string EmailPropertyName = "Email";
        private const string UserNamePropertyName = "UserName";
        private const string PasswordPropertyName = "Password";

        private static string? ResolvePropertyName(string code)
        {
            return code switch
            {
                "DuplicateEmail" => EmailPropertyName,
                "DuplicateUserName" => UserNamePropertyName,
                "InvalidEmail" => EmailPropertyName,

                "PasswordTooShort" => PasswordPropertyName,
                "PasswordRequiresDigit" => PasswordPropertyName,
                "PasswordRequiresUpper" => PasswordPropertyName,
                "PasswordRequiresLower" => PasswordPropertyName,
                "PasswordRequiresNonAlphanumeric" => PasswordPropertyName,
                "PasswordRequiresUniqueChars" => PasswordPropertyName,

                _ => null
            };
        }
    }
}