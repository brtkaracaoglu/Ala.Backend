using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.UserRoles;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.UserRoles.GetPagedUserRoles
{
    public class GetPagedUserRolesQueryHandler : IRequestHandler<GetPagedUserRolesQueryRequest, SuccessDetails<PagedResponse<UserRolesDto>>>
    {
        private readonly IUserRoleQueryService _userRoleQueryService;

        public GetPagedUserRolesQueryHandler(IUserRoleQueryService userRoleQueryService)
        {
            _userRoleQueryService = userRoleQueryService;
        }

        public async Task<SuccessDetails<PagedResponse<UserRolesDto>>> Handle(GetPagedUserRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _userRoleQueryService.GetPagedAsync(request, cancellationToken);

            return ResultResponse.Success(result, Response.Common.OperationSuccess);
        }
    }
}