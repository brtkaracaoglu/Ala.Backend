using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.PermissionAssignment.GetRolePermission
{
    public class GetRolePermissionsQueryHandler : IRequestHandler<GetRolePermissionsQueryRequest, SuccessDetails<IEnumerable<string>>>
    {
        private readonly IRoleService _roleService;

        public GetRolePermissionsQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<SuccessDetails<IEnumerable<string>>> Handle(GetRolePermissionsQueryRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleService.FindByIdAsync(request.RoleId.ToString());

            if (role is null)
                throw new NotFoundException("Rol bulunamadı.");

            var claims = await _roleService.GetClaimsAsync(role);

            var permissions = claims.Where(x => x.Type == "permission").Select(x => x.Value);

            return ResultResponse.Success(permissions, Response.Common.OperationSuccess);
        }
    }
}