using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.LockUser
{
    public class LockUserCommandRequest : IRequest<SuccessDetails>
    {
        public int Id { get; set; }
        public int? DurationInHours { get; set; }
    }
}
