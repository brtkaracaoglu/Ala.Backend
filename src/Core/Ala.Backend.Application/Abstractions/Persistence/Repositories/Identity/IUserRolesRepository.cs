using Ala.Backend.Application.DTOs.UserRoles;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity
{
    public interface IUserRolesRepository
    {
        Task<IList<UserRolesDto>> GetAllUserRolesAsync(CancellationToken cancellationToken = default);
    }
}