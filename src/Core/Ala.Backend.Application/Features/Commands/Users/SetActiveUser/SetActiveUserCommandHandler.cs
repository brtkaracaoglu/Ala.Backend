using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.SetActiveUser
{
    public class SetActiveUserCommandHandler : IRequestHandler<SetActiveUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;

        public SetActiveUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails> Handle(SetActiveUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.Id.ToString());
            if (user is null)
                throw new NotFoundException("Durumu güncellenmek istenen kullanıcı sistemde bulunamadı.");

            if (user.IsActive == request.IsActive)
                return ResultResponse.Success(Response.Common.OperationSuccess);
                      
            user.IsActive = request.IsActive;
            var result = await _userService.UpdateAsync(user); 

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Kullanıcı durumu güncellenirken hata meydana geldi: {errors}");
            }

            if (!request.IsActive)
            {
                await _userService.UpdateSecurityStampAsync(user);
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}