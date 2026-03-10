using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.DeleteUser
{
    public class DeleteUserCommandRequest :IRequest<SuccessDetails>
    {
        public int Id { get; set; }
    }
}
