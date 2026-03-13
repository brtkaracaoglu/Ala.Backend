using Ala.Backend.Domain.Identity;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity
{
    public interface IRoleRepository
    {
        Task<IList<Role>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    }
}
