using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using Ala.Backend.Domain.Identity;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Roles.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommandRequest, SuccessDetails<int>>
    {
        private readonly IRoleService _roleService;

        public CreateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<SuccessDetails<int>> Handle(CreateRoleCommandRequest request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleService.FindByNameAsync(request.Name);
            if (existingRole is not null)
                throw new ConflictException("Bu rol adı sistemde zaten kayıtlı.", nameof(request.Name));

            var role = new Role { Name = request.Name };
            var result = await _roleService.CreateAsync(role);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new OperationFailedException($"Rol oluştururken hata meydana geldi: {errors}");
            }

            return ResultResponse.Created(role.Id, Response.Common.OperationSuccess);
        }
    }
}
