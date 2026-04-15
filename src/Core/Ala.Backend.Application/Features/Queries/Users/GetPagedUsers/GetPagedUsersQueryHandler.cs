using Ala.Backend.Application.Abstractions.Persistence.Service.Queries;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Users.GetPagedUsers
{
    public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQueryRequest, SuccessDetails<PagedResponse<UserDto>>>
    {
        private readonly IUserQueryService _userQueryService;

        public GetPagedUsersQueryHandler(IUserQueryService userQueryService)
        {
            _userQueryService = userQueryService;
        }

        public async Task<SuccessDetails<PagedResponse<UserDto>>> Handle(GetPagedUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var result = await _userQueryService.GetPagedAsync(request, cancellationToken);

            return ResultResponse.Success(result, Response.Common.OperationSuccess);
        }
    }
}