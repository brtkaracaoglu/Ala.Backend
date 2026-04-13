using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.RemovePermissionFromUser
{
    public class RemovePermissionFromUserCommandRequest : IRequest<SuccessDetails>
    {
        public int UserId { get; set; }
        public string PermissionCode { get; set; } = null!;

    }
}
