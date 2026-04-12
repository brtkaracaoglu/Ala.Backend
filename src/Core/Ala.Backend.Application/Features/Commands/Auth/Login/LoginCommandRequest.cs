using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.Login
{
    public class LoginCommandRequest : IRequest<SuccessDetails<LoginCommandResult>>
    {
        public string EmailOrUsername { get; set; } = null!;
        public string Password { get; set; } = null!;

    }
}
