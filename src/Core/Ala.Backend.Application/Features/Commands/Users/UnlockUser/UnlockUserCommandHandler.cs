using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.UnlockUser
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;

        public UnlockUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails> Handle(UnlockUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.Id.ToString());
            if (user is null)
                throw new DirectoryNotFoundException("Kilidi açılmakk istenen kullanıcı sistemde bulunamadi");

            var result = await _userService.SetLockoutEndDateAsync(user, null);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Kullanıcı kilidi açılırken hata meydana geldi: {errors}");
            }

            await _userService.ResetAccessFailedCountAsync(user);

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}
