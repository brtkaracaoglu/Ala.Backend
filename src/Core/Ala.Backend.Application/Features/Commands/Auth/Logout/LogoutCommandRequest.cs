using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.Logout
{
    public class LogoutCommandRequest : IRequest<SuccessDetails<LogoutCommandResult>>
    {
        public string? RefreshToken { get; set; }
    }
}