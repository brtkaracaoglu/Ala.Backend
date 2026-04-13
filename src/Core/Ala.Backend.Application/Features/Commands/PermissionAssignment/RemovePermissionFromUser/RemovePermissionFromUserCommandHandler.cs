using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.RemovePermissionFromUser
{
    public class RemovePermissionFromUserCommandHandler : IRequestHandler<RemovePermissionFromUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;

        public RemovePermissionFromUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails> Handle(RemovePermissionFromUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                throw new NotFoundException("Kullanıcı bulunamadı.");

            var claims = await _userService.GetClaimsAsync(user);
            var targetClaim = claims.FirstOrDefault(x => x.Type == "permission" && x.Value == request.PermissionCode);

            if (targetClaim is null)
                throw new NotFoundException("Kullanıcıya atanmış böyle bir yetki bulunamadı.");

            var result = await _userService.RemoveClaimAsync(user, targetClaim);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Yetki kullanıcıdan kaldırılırken bir hata oluştu: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}