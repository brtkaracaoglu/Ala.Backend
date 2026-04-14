using Ala.Backend.Application.Common.Requests;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetRoles
{
    public class GetRolesQueryRequest : PagedQueryRequest, IRequest<SuccessDetails<PagedResponse<RoleDto>>>
    {
    }
}