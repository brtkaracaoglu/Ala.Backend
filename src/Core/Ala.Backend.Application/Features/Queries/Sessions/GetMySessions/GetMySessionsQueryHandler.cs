using Ala.Backend.Application.Abstractions.Infrastructure.Services.Sessions;
using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.SystemMessages;
using MediatR;

namespace Ala.Backend.Application.Features.Queries.Sessions.GetMySessions
{
    public class GetMySessionsQueryHandler : IRequestHandler<GetMySessionsQueryRequest, SuccessDetails<List<UserSessionDto>>>
    {
        private readonly IRequestContext _requestContext;
        private readonly IUserSessionService _userSessionService;
        private readonly IClientInfoParser _clientInfoParser;

        public GetMySessionsQueryHandler(
            IRequestContext requestContext,
            IUserSessionService userSessionService,
            IClientInfoParser clientInfoParser)
        {
            _requestContext = requestContext;
            _userSessionService = userSessionService;
            _clientInfoParser = clientInfoParser;
        }

        public async Task<SuccessDetails<List<UserSessionDto>>> Handle(
            GetMySessionsQueryRequest request,
            CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            var currentFamilyId = request.CurrentSessionFamilyId;

            var sessions = await _userSessionService.GetUserSessionsAsync(
                _requestContext.UserId.Value,
                request.Filter,
                cancellationToken);

            var orderedSessions = sessions
                .OrderByDescending(x => currentFamilyId.HasValue && currentFamilyId.Value == x.FamilyId)
                .ThenByDescending(x => x.IsActive)
                .ThenByDescending(x => x.LastActivityOnUtc)
                .ToList();

            var dto = orderedSessions.Select(x =>
            {
                var parsedClientInfo = _clientInfoParser.Parse(x.CreatedByUserAgent);
                var isCurrentSession = currentFamilyId.HasValue && currentFamilyId.Value == x.FamilyId;

                return new UserSessionDto
                {
                    Id = x.Id,
                    FamilyId = x.FamilyId,
                    IpAddress = x.CreatedByIp,
                    UserAgent = x.CreatedByUserAgent ?? "Unknown",
                    Browser = parsedClientInfo.Browser,
                    Platform = parsedClientInfo.Platform,
                    Device = parsedClientInfo.Device,
                    DisplayName = parsedClientInfo.DisplayName,
                    CreatedOnUtc = x.CreatedOnUtc,
                    LastActivityOnUtc = x.LastActivityOnUtc,
                    IsActive = x.IsActive,
                    RevokedAtUtc = x.RevokedAtUtc,
                    ReasonRevoked = x.ReasonRevoked,
                    IsCurrent = isCurrentSession
                };
            }).ToList();

            return ResultResponse.Success(dto, Response.Common.OperationSuccess);
        }
    }
}