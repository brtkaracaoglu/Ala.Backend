using Ala.Backend.Application.Common.Requests;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Users.GetUsers
{
    public sealed class GetUsersQueryRequest : PagedQueryRequest, IRequest<SuccessDetails<PagedResponse<UserDto>>>
    {
        public bool? IsActive { get; set; }
        public string? Role { get; set; }
    }
}
