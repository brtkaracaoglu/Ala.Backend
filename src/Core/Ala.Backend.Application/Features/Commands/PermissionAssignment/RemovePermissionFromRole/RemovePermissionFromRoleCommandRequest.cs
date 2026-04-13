using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleCommandRequest : IRequest<SuccessDetails>
    {
        public int RoleId { get; set; }
        public string PermissionCode { get; set; } = null!;

    }
}
