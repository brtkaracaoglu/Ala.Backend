using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ChangePassword
{
    public class ChangePasswordCommandRequest : IRequest<SuccessDetails<LogoutCommandResult>>
    {
        public string OldPassword { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
        public string ConfirmNewPassword { get; set; } = null!;
    }
}