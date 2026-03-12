using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetAllRoles
{
    public class GetAllRolesQueryRequest : IRequest<SuccessDetails<List<RoleDto>>>
    {
    }
}
