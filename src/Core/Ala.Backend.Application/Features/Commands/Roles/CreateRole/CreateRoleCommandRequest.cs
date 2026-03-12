using Ala.Backend.Application.Common.Responses;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Roles.CreateRole
{
    public class CreateRoleCommandRequest : IRequest<SuccessDetails<int>>
    {
        public string Name { get; set; }
    }
}
