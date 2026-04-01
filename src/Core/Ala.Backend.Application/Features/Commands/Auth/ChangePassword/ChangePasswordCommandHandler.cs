using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Auth.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;
        private readonly IRequestContext _requestContext;

        public ChangePasswordCommandHandler(IUserService userService, IRequestContext requestContext)
        {
            _userService = userService;
            _requestContext = requestContext;
        }

        public async Task<SuccessDetails> Handle(ChangePasswordCommandRequest request, CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            var user = await _userService.FindByIdAsync(_requestContext.UserId.Value.ToString());

            if (user is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            var check = await _userService.CheckPasswordSignInAsync(user, request.OldPassword, lockoutOnFailure: false);

            if (!check.Succeeded)
                throw new UnauthorizedException("Mevcut şifre hatalı.");

            var result = await _userService.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Şifre değiştirme işlemi başarısız oldu: {errors}");
            }

            await _userService.UpdateSecurityStampAsync(user);

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}