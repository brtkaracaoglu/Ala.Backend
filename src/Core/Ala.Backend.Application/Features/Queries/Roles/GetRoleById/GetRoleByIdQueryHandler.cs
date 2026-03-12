using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetRoleById
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQueryRequest, SuccessDetails<RoleDto>>
    {
        private readonly IRoleService _roleService;

        public GetRoleByIdQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<SuccessDetails<RoleDto>> Handle(GetRoleByIdQueryRequest request, CancellationToken cancellationToken)
        {
            var role = await _roleService.FindByIdAsync(request.Id.ToString());

            if (role is null)
                throw new NotFoundException("Role bulunamadı.");

            var roleDto = new RoleDto { Id = role.Id, Name = role.Name! };

            return ResultResponse.Success(roleDto, Response.Common.OperationSuccess);
        }
    }
}
