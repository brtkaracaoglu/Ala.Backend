using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ForgotPassword
{
    public class ForgotPasswordCommandRequest : IRequest<SuccessDetails>
    {
        public string Email { get; set; } = string.Empty;

    }
}
