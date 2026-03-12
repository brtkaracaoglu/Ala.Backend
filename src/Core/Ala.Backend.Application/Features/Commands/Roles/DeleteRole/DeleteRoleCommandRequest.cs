using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Roles.DeleteRole
{
    public class DeleteRoleCommandRequest : IRequest<SuccessDetails>
    {
        public int Id { get; set; }
    }
}
