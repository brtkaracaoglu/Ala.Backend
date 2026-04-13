using Ala.Backend.WebAPI.Authorization.Permissions;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Ala.Backend.WebAPI.Startup
{
    public static class PermissionScanner
    {
        public static IEnumerable<string> ScanControllers(Assembly assembly)
        {
            var permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var controllerType in GetControllerTypes(assembly))
            {
                AddPermissions(permissions, GetPermissionsFromMember(controllerType));

                foreach (var method in GetControllerMethods(controllerType))
                {
                    AddPermissions(permissions, GetPermissionsFromMember(method));
                }
            }

            return permissions;
        }

        private static IEnumerable<Type> GetControllerTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && !t.IsAbstract);
        }

        private static IEnumerable<MethodInfo> GetControllerMethods(Type controllerType)
        {
            return controllerType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(m => !m.IsSpecialName)
                .Where(m => !m.IsDefined(typeof(NonActionAttribute), inherit: true));
        }

        private static IEnumerable<string> GetPermissionsFromMember(MemberInfo member)
        {
            return member
                .GetCustomAttributes<PermissionAuthorizeAttribute>(inherit: true)
                .Select(attr => attr.Permission?.Trim())
                .Where(permission => !string.IsNullOrWhiteSpace(permission))!;
        }

        private static void AddPermissions(HashSet<string> permissions, IEnumerable<string> values)
        {
            foreach (var value in values)
            {
                permissions.Add(value);
            }
        }
    }
}