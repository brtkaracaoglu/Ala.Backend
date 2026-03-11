using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.LockUser
{
    public class LockUserCommandHandler : IRequestHandler<LockUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;

        public LockUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails> Handle(LockUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.Id.ToString());
            if (user is null)
                throw new NotFoundException("Kilitlenmek istenen kullanıcı sistemde bulunamadı.");

            await _userService.SetLockoutEnabledAsync(user, true);

            DateTimeOffset lockoutEndDate = request.DurationInHours.HasValue
                ? DateTimeOffset.UtcNow.AddHours(request.DurationInHours.Value)
                : DateTimeOffset.MaxValue;

            var result = await _userService.SetLockoutEndDateAsync(user, lockoutEndDate);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Kullanıcı kilitlenirken hata meydana geldi: {errors}");
            }

            await _userService.UpdateSecurityStampAsync(user);

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}