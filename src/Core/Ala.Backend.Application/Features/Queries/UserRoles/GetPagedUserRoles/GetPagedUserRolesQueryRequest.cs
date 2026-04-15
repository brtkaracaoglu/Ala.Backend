using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.UserRoles;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.UserRoles.GetPagedUserRoles
{
    public class GetPagedUserRolesQueryRequest : UserRoleListFilter, IRequest<SuccessDetails<PagedResponse<UserRolesDto>>>
    {
    }
}