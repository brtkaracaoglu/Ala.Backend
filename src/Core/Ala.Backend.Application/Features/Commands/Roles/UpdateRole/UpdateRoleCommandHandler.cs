using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Roles.UpdateRole
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommandRequest, SuccessDetails>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<SuccessDetails> Handle(UpdateRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleService.FindByIdAsync(request.Id.ToString());
            if (role is null)
                throw new NotFoundException("Güncellenmek istenen rol sistemde bulunamadı.");

            if (role.Name != request.Name) 
            {
                var roleNameExists = await _roleService.FindByNameAsync(request.Name);
                if (roleNameExists is not null)
                    throw new ConflictException("Bu rol adı zaten sistemde mevcut.");
            }

            role.Name = request.Name;
            var result = await _roleService.UpdateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Rol güncellenirken hata oluştu: {errors}");
            }

            return ResultResponse.Success(Response.Common.OperationSuccess);
        }
    }
}
