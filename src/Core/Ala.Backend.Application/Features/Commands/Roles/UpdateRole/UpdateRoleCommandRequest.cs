using Ala.Backend.Application.Common.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace Ala.Backend.Application.Features.Commands.Roles.UpdateRole
{
    public class UpdateRoleCommandRequest : IRequest<SuccessDetails>
    {
        [JsonIgnore] 
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
