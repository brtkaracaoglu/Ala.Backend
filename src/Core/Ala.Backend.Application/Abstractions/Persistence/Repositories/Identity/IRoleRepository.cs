using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;

namespace Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity
{
    public interface IRoleRepository
    {
        Task<PagedResponse<RoleDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    }
}
