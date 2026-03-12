using Ala.Backend.Application.Common.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace Ala.Backend.Application.Features.Commands.Users.SetActiveUser
{
    public class SetActiveUserCommandRequest : IRequest<SuccessDetails>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public bool IsActive { get; set; }
    }
}