using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.UserRoles;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.UserRoles.GetAllUserRoles
{
    public class GetAllUserRolesQueryRequest : IRequest<SuccessDetails<List<UserRolesDto>>>
    {
    }
}