namespace Ala.Backend.Application.Abstractions.Persistence.Seeders
{
    public interface IPermissionSeeder
    {
        Task SyncPermissionsAsync(IEnumerable<string> permissionCodes);
    }
}
