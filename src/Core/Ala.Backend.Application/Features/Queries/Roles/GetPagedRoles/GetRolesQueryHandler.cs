using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetPagedRoles
{
    public class GetPagedRolesQueryHandler : IRequestHandler<GetPagedRolesQueryRequest, SuccessDetails<PagedResponse<RoleDto>>>
    {
        private readonly IRoleQueryService _roleQueryService;

        public GetPagedRolesQueryHandler(IRoleQueryService roleQueryService)
        {
            _roleQueryService = roleQueryService;
        }

        public async Task<SuccessDetails<PagedResponse<RoleDto>>> Handle(GetPagedRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _roleQueryService.GetPagedAsync(request, cancellationToken);

            return ResultResponse.Success(result, Response.Common.OperationSuccess);
        }
    }
}