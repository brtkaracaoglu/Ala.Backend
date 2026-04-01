using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;
using System.Text.Json.Serialization;

namespace Ala.Backend.Application.Features.Commands.Auth.RefreshToken
{
    public class RefreshTokenCommandRequest : IRequest<SuccessDetails<LoginResponseDto>>
    {
        public string RefreshToken { get; set; } = null!;

        [JsonIgnore]
        public string IpAddress { get; set; } = "N/A";
    }
}
