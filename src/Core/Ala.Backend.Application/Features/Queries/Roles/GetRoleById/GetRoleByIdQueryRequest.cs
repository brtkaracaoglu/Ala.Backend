using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Roles;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Roles.GetRoleById
{
    public class GetRoleByIdQueryRequest : IRequest<SuccessDetails<RoleDto>>
    {
        public int Id { get; set; }
    }
}
