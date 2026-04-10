using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Commands.Sessions.RevokeSession
{
    public class RevokeSessionCommandRequest : IRequest<SuccessDetails<RevokeSessionResponseDto>>
    {
        public long SessionId { get; set; }
        public Guid? CurrentSessionFamilyId { get; set; }
    }
}