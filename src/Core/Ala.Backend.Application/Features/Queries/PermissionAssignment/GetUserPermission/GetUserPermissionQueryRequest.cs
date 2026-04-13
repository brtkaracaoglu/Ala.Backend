using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.PermissionAssignment.GetUserPermission
{
    public class GetUserPermissionsQueryRequest : IRequest<SuccessDetails<IEnumerable<string>>>
    {
        public int UserId { get; set; }
    }
}
