using Ala.Backend.WebAPI.Authorization.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Ala.Backend.WebAPI.Authorization
{
    public static class AuthorizationServiceRegistration
    {
        public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}