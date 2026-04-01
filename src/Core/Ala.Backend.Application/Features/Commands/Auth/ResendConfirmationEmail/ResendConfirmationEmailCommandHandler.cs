using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.Extensions;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ResendConfirmationEmail
{
    public sealed class ResendConfirmationEmailCommandHandler : IRequestHandler<ResendConfirmationEmailCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;
        private readonly IMailService _mailService;

        public ResendConfirmationEmailCommandHandler(IUserService userService, IMailService mailService)
        {
            _userService = userService;
            _mailService = mailService;
        }

        public async Task<SuccessDetails> Handle(ResendConfirmationEmailCommandRequest request, CancellationToken cancellationToken)
        {
            var normalizedEmail = NormalizeEmail(request.Email);

            var user = await _userService.FindByEmailAsync(normalizedEmail);

            if (user is not null && user.IsActive && !user.EmailConfirmed)
            {
                var token = await _userService.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = TokenExtensions.EncodeToken(token);

                var displayName = string.IsNullOrWhiteSpace(user.UserName)
                    ? $"{user.FirstName} {user.LastName}".Trim()
                    : user.UserName;

                await _mailService.SendResendConfirmationMailAsync(normalizedEmail, displayName!, encodedToken);
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }

        private static string NormalizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return string.Empty;

            return email.Trim().ToLowerInvariant();
        }
    }
}