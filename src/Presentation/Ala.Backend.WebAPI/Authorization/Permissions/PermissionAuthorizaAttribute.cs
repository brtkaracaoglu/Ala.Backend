using Microsoft.AspNetCore.Authorization;

namespace Ala.Backend.WebAPI.Authorization.Permissions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public PermissionAuthorizeAttribute(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission))
                throw new ArgumentException("Permission cannot be null or empty.", nameof(permission));

            Policy = PermissionPolicyConstants.Prefix + permission;
        }

        public string Permission => Policy![PermissionPolicyConstants.Prefix.Length..];
    }
}