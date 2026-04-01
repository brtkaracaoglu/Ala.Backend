using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ResendConfirmationEmail
{
    public class ResendConfirmationEmailCommandRequest : IRequest<SuccessDetails>
    {
        public string Email { get; set; } = string.Empty;

    }
}
