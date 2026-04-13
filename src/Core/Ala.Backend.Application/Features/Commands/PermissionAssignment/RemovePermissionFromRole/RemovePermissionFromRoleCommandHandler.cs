using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.RemovePermissionFromRole
{
    public class RemovePermissionFromRoleCommandHandler : IRequestHandler<RemovePermissionFromRoleCommandRequest, SuccessDetails>
    {
        private readonly IRoleService _roleService;

        public RemovePermissionFromRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }
        public async Task<SuccessDetails> Handle(RemovePermissionFromRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleService.FindByIdAsync(request.RoleId.ToString());
            if (role is null)
                throw new NotFoundException("Rol Bulunamadı.");

            var claims = await _roleService.GetClaimsAsync(role);
            var targetClaim = claims.FirstOrDefault(c => c.Type == "permission" && c.Value == request.PermissionCode);

            if (targetClaim is null)
                throw new NotFoundException("Rol üzerinde belirtilen izin bulunamadı.");
            var result = await _roleService.RemoveClaimAsync(role, targetClaim);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Yetki rolden kaldırılırken bir hata oluştu: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);

        }
    }
}
