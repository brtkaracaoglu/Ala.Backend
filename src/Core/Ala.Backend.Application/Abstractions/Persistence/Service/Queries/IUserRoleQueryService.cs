using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.UserRoles;

namespace Ala.Backend.Application.Abstractions.Persistence.Service.Queries
{
    public interface IUserRoleQueryService
    {
        Task<PagedResponse<UserRolesDto>> GetPagedAsync(UserRoleListFilter filter, CancellationToken cancellationToken = default);
    }
}