using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetRoles
{
    public class GetRolesQueryHandler : IRequestHandler<GetRolesQueryRequest, SuccessDetails<PagedResponse<RoleDto>>>
    {
        private readonly IRoleRepository _roleRepository;

        public GetRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<SuccessDetails<PagedResponse<RoleDto>>> Handle(GetRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleRepository.GetPagedAsync(request.Page, request.PageSize, cancellationToken);

            return ResultResponse.Success(result, Response.Common.OperationSuccess);
        }
    }
}