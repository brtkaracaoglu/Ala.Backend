using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Abstractions.Persistence;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using Ala.Backend.Domain.Identity;
using MediatR;
using System.Security.Claims;

namespace Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToRole
{
    public class AssignPermissionToRoleCommandHandler : IRequestHandler<AssignPermissionToRoleCommandRequest, SuccessDetails>
    {
        private readonly IRoleService _roleService;
        private readonly IUnitOfWork _unitOfWork;

        public AssignPermissionToRoleCommandHandler(IRoleService roleService, IUnitOfWork unitOfWork)
        {
            _roleService = roleService;
            _unitOfWork = unitOfWork;
        }

        public async Task<SuccessDetails> Handle(AssignPermissionToRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var permissionRepo = _unitOfWork.ReadRepository<Permission, int>();

            var permissionExists = await permissionRepo.AnyAsync(x => x.Code == request.PermissionCode, cancellationToken);
            if (!permissionExists)
                throw new NotFoundException("Belirtilen yetki kodu sistemde bulunamadı.");

            var role = await _roleService.FindByIdAsync(request.RoleId.ToString());
            if (role is null)
                throw new NotFoundException("Rol bulunamadı.");

            var result = await _roleService.AddClaimAsync(role, new Claim("permission", request.PermissionCode));

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Yetki atanırken bir hata oluştu: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}