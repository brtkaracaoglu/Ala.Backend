using Ala.Backend.Application.Common.Responses;
using MediatR;
using System.Text.Json.Serialization;

namespace Ala.Backend.Application.Features.Commands.Users.UpdateUser
{
    public class UpdateUserCommandRequest : IRequest<SuccessDetails>
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }
}