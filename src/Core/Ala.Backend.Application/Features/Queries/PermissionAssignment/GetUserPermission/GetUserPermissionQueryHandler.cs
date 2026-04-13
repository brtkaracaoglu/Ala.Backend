using Ala.Backend.Application.Abstractions.Infrastructure.Services.Identity;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.PermissionAssignment.GetUserPermission
{
    public class GetUserPermissionsQueryHandler : IRequestHandler<GetUserPermissionsQueryRequest, SuccessDetails<IEnumerable<string>>>
    {
        private readonly IUserService _userService;

        public GetUserPermissionsQueryHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<SuccessDetails<IEnumerable<string>>> Handle(GetUserPermissionsQueryRequest request, CancellationToken cancellationToken)
        {
            var user = await _userService.FindByIdAsync(request.UserId.ToString());

            if (user is null)
                throw new NotFoundException("User bulunamadı.");

            var claims = await _userService.GetClaimsAsync(user);
            var permissions = claims.Where(x => x.Type == "permission").Select(x => x.Value);

            return ResultResponse.Success(permissions, Response.Common.OperationSuccess);
        }
    }
}
