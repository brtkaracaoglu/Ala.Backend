using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.UserRoles.AddRoleToUser
{
    public class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AddRoleToUserCommandHandler(IRoleService roleService, IUserService userService)
        {
            _roleService = roleService;
            _userService = userService;
        }

        public async Task<SuccessDetails> Handle(AddRoleToUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                throw new NotFoundException("Kullanıcı sistemde bulunamadı.");

            var role = await _roleService.FindByIdAsync(request.RoleId.ToString());
            if (role is null)
                throw new NotFoundException("Atanmak istenen rol sistemde bulunamadı.");

            var isInRole = await _userService.IsInRoleAsync(user, role.Name!);
            if (isInRole)
                throw new OperationFailedException("Kullanıcı zaten bu role sahip.");

            var result = await _userService.AddToRoleAsync(user, role.Name!);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Rol atama işlemi başarısız: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}