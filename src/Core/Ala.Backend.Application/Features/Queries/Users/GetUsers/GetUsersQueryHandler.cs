using Ala.Backend.Application.Abstractions.Persistence.Repositories.Identity;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Users.GetUsers
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQueryRequest, SuccessDetails<PagedResponse<UserDto>>>
    {
        private readonly IUserRepository _userRepository;

        public GetUsersQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<SuccessDetails<PagedResponse<UserDto>>> Handle(GetUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var filter = new UserListFilter
            {
                Page = request.Page,
                PageSize = request.PageSize,
                Search = request.Search?.Trim(),
                SortBy = request.SortBy,
                SortDirection = request.SortDirection,
                IsActive = request.IsActive,
                Role = request.Role?.Trim()
            };

            var result = await _userRepository.GetPagedUsersWithRolesAsync(filter, cancellationToken);

            return ResultResponse.Success(result, Response.Common.OperationSuccess);
        }
    }
}