using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;

        public ForgotPasswordCommandHandler(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
        }

        public async Task<SuccessDetails> Handle(ForgotPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByEmailAsync(request.Email);

            if (user is not null && user.EmailConfirmed && user.IsActive)
            {
                var token = await _userService.GeneratePasswordResetTokenAsync(user);
                var encodedToken = System.Web.HttpUtility.UrlEncode(token);
                var fullName = $"{user.FirstName} {user.LastName}".Trim();

                await _mailService.SendPasswordResetMailAsync(user.Email!, user.Id, fullName, encodedToken);
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}