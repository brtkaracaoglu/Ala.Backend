using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ConfirmEmail
{
    public class ConfirmEmailCommandRequest : IRequest<SuccessDetails>
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;

    }
}
