using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Users;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Users.GetUserById
{
    public class GetUserByIdQueryRequest : IRequest<SuccessDetails<UserDto>>
    {
        public int Id { get; set; }
    }
}