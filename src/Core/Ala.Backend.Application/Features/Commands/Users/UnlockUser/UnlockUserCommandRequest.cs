using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.UnlockUser
{
    public class UnlockUserCommandRequest : IRequest<SuccessDetails>
    {
        public int Id { get; set; }
    }
}
