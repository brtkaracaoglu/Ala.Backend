using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Auth.GetCurrentUser
{
    public class GetCurrentUserQueryRequest : IRequest<SuccessDetails<CurrentUserDto>>
    {
    }
}