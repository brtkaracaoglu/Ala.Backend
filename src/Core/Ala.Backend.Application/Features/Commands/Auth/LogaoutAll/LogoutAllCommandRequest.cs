using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.LogoutAll
{
    public class LogoutAllCommandRequest : IRequest<SuccessDetails<LogoutCommandResult>>
    {
    }
}