using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Permissions;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Permissions.GetPagedPermissions
{
    public class GetPagedPermissionsQueryRequest : PermissionListFilter, IRequest<SuccessDetails<PagedResponse<PermissionDto>>>
    {
    }
}
