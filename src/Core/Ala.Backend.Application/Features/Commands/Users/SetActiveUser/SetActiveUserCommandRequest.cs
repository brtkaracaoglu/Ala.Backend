using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.SetActiveUser
{
    public class SetActiveUserCommandRequest : IRequest<SuccessDetails>
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}