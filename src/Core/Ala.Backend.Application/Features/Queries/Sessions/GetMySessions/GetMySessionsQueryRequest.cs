using Ala.Backend.Application.Common.Enums;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Sessions.GetMySessions
{
    public class GetMySessionsQueryRequest : IRequest<SuccessDetails<List<UserSessionDto>>>
    {
        public SessionFilterType Filter { get; set; } = SessionFilterType.All;
        public Guid? CurrentSessionFamilyId { get; set; }
    }
}