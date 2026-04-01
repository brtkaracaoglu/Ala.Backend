using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.Logout
{
    public class LogoutCommandRequest : IRequest<SuccessDetails>
    {
        public int UserId { get; set; }

    }
}
