using Microsoft.AspNetCore.Authorization;

namespace Ala.Backend.WebAPI.Authorization.Permissions
{
    public sealed class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("Permission cannot be null or empty.", nameof(permission));

            Permission = permission;
        }
    }
}
