using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommandRequest : IRequest<SuccessDetails>
    {
        public int RoleId { get; set; }
        public string PermissionCode { get; set; } = null!;

    }
}
