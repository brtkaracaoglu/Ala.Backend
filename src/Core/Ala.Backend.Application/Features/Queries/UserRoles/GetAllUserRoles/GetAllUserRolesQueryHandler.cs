using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.UserRoles;
using Ala.Backend.Application.SystemMessages; 
using MediatR;

namespace Ala.Backend.Application.Features.Queries.UserRoles.GetAllUserRoles
{
    public class GetAllUserRolesQueryHandler : IRequestHandler<GetAllUserRolesQueryRequest, SuccessDetails<List<UserRolesDto>>>
    {
        private readonly IUserRolesRepository _userRolesRepository;

        public GetAllUserRolesQueryHandler(IUserRolesRepository userRolesRepository)
        {
            _userRolesRepository = userRolesRepository;
        }

        public async Task<SuccessDetails<List<UserRolesDto>>> Handle(GetAllUserRolesQueryRequest request, CancellationToken cancellationToken)
        {
            var userRoles = await _userRolesRepository.GetAllUserRolesAsync(cancellationToken);

            var response = userRoles.ToList();
            return ResultResponse.Success(response, Response.Common.OperationSuccess);
        }
    }
}