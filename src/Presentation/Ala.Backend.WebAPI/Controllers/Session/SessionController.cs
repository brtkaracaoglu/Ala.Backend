using Ala.Backend.Application.Abstractions.Infrastructure.Services.Token;
using Ala.Backend.Application.Features.Commands.Sessions.RevokeSession;
using Ala.Backend.Application.Features.Queries.Sessions.GetMySessions;
using Ala.Backend.Presentation.Abstractions;
using Ala.Backend.WebAPI.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.Session
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Session")]
    public class SessionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenCookieService _tokenCookieService;
        private readonly ITokenRevocationService _tokenRevocationService;

        public SessionController(
            IMediator mediator,
            ITokenCookieService tokenCookieService,
            ITokenRevocationService tokenRevocationService)
        {
            _mediator = mediator;
            _tokenCookieService = tokenCookieService;
            _tokenRevocationService = tokenRevocationService;
        }

        [Authorize]
        [HttpGet("sessions")]
        public async Task<IActionResult> GetMySessions(
            [FromQuery] GetMySessionsQueryRequest request,
            CancellationToken cancellationToken)
        {
            request.CurrentSessionFamilyId = await GetCurrentSessionFamilyIdAsync(cancellationToken);

            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [ServiceFilter(typeof(TokenCookieFilter))]
        [HttpDelete("sessions/{sessionId:long}")]
        public async Task<IActionResult> RevokeSession(
            [FromRoute] RevokeSessionCommandRequest request,
            CancellationToken cancellationToken)
        {
            request.CurrentSessionFamilyId = await GetCurrentSessionFamilyIdAsync(cancellationToken);

            var result = await _mediator.Send(request, cancellationToken);
            return Ok(result);
        }

        private async Task<Guid?> GetCurrentSessionFamilyIdAsync(CancellationToken cancellationToken)
        {
            var refreshToken = _tokenCookieService.GetRefreshToken();

            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            return await _tokenRevocationService.GetFamilyIdByRefreshTokenAsync(
                refreshToken,
                cancellationToken);
        }
    }
}