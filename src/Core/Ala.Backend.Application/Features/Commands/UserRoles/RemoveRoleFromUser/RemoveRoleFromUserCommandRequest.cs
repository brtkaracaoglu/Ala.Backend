using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.UserRoles.RemoveRoleFromUser
{
    public class RemoveRoleFromUserCommandRequest : IRequest<SuccessDetails>
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
