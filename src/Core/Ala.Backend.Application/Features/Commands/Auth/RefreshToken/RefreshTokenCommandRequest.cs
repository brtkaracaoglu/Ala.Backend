using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandRequest : IRequest<SuccessDetails<RefreshTokenCommandResult>>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}