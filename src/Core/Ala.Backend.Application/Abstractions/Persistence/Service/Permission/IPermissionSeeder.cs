namespace Ala.Backend.Application.Abstractions.Persistence.Service.Permission
{
    public interface IPermissionSeeder
    {
        Task SyncPermissionsAsync(IEnumerable<string> permissionCodes);
    }
}
