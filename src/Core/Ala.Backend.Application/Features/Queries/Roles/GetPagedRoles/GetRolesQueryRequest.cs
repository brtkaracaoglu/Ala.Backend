using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetPagedRoles
{
    public class GetPagedRolesQueryRequest : RoleListFilter, IRequest<SuccessDetails<PagedResponse<RoleDto>>>
    {
    }
}