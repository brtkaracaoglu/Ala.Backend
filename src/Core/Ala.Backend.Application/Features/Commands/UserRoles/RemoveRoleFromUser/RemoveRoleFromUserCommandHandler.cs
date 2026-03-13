using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.UserRoles.RemoveRoleFromUser
{
    public class RemoveRoleFromUserCommandHandler : IRequestHandler<RemoveRoleFromUserCommandRequest, SuccessDetails>
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public RemoveRoleFromUserCommandHandler(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        public async Task<SuccessDetails> Handle(RemoveRoleFromUserCommandRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.UserId.ToString());
            if (user is null)
                throw new NotFoundException("Kullanıcı sistemde bulunamadı.");

            var role = await _roleService.FindByIdAsync(request.RoleId.ToString());
            if (role is null)
                throw new NotFoundException("Sistemde böyle bir rol bulunamadı.");

            var isInRole = await _userService.IsInRoleAsync(user, role.Name!);
            if (!isInRole)
                throw new OperationFailedException("Kullanıcı zaten bu role sahip değil.");

            var result = await _userService.RemoveFromRoleAsync(user, role.Name!);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Rol kaldırılırken hata oluştu: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}