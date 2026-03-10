using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Users.GetAllUsers
{
    public class GetAllUsersQueryRequest : IRequest<SuccessDetails<List<UserDto>>>
    {
    }
}