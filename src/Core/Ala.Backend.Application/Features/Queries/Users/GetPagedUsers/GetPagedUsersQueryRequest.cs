using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Users.GetPagedUsers
{
    public class GetPagedUsersQueryRequest : UserListFilter, IRequest<SuccessDetails<PagedResponse<UserDto>>>
    {
    }
}