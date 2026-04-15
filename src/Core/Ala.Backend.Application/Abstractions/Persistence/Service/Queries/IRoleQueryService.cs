using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;

namespace Ala.Backend.Application.Abstractions.Persistence.Service.Queries
{
    public interface IRoleQueryService
    {
        Task<PagedResponse<RoleDto>> GetPagedAsync(RoleListFilter request, CancellationToken cancellationToken = default);
    }
}
