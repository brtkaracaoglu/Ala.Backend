using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.Id.ToString());
            if (user is null)
                throw new NotFoundException("Güncellenmek istenen kullanıcı sistemde bulunamadı.");

            if (user.Email != request.Email)
            {
                var emailExists = await _userService.FindByEmailAsync(request.Email);
                if (emailExists is not null)
                    throw new ConflictException("Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.", nameof(request.Email));
            }

            if (user.UserName != request.UserName)
            {
                var userNameExists = await _userService.FindByNameAsync(request.UserName);
                if (userNameExists is not null)
                   throw new ConflictException("Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.", nameof(request.UserName));
                
            }

            user.UserName = request.UserName;
            user.Email = request.Email;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            var result = await _userService.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Kullanıcı güncellenirken hata meydana geldi: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}
