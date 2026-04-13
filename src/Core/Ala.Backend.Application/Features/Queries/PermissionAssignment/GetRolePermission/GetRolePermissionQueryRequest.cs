using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.PermissionAssignment.GetRolePermission
{
    public class GetRolePermissionsQueryRequest : IRequest<SuccessDetails<IEnumerable<string>>>
    {
        public int RoleId { get; set; }
    }
}
