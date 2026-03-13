using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetAllRoles
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQueryRequest, SuccessDetails<List<RoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetAllRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<SuccessDetails<List<RoleDto>>> Handle(GetAllRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllRolesAsync(cancellationToken);

            var roleDtos = roles.Select(role => new RoleDto
            {
                Id = role.Id,
                Name = role.Name!
            }).ToList();    

            return ResultResponse.Success(roleDtos, Response.Common.OperationSuccess);

        }
    }
}
