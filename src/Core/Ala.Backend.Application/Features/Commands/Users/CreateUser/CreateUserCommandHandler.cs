using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using Ala.Backend.Domain.Identity;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Users.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, SuccessDetails<int>>
    {
        private readonly IUserService _userService;

        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails<int>> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var existingEmail = await _userService.FindByEmailAsync(request.Email);
            if (existingEmail is not null)
                throw new ConflictException("Bu e-posta adresi sistemde zaten kayıtlı.", nameof(request.Email));

            var existingUserName = await _userService.FindByNameAsync(request.UserName);
            if (existingUserName is not null)
                throw new ConflictException("Bu kullanıcı adı sistemde zaten kayıtlı.", nameof(request.UserName));

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                IsActive = true
            };

            var result = await _userService.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Kullanıcı oluşturulurken hata meydana geldi: {errors}");
            }

            return ResultResponse.Created(user.Id, Response.Common.OperationSuccess);
        }
    }
}
