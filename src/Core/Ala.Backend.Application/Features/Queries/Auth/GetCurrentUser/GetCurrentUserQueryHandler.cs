using Ala.Backend.Application.Abstractions.Presentation;
using Ala.Backend.Application.Common.Exceptions;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.DTOs.Auth;
using Ala.Backend.Application.SystemMessages;
using MediatR;
using System.Security.Claims;

namespace Ala.Backend.Application.Features.Queries.Auth.GetCurrentUser
{
    public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQueryRequest, SuccessDetails<CurrentUserDto>>
    {
        private readonly IRequestContext _requestContext;

        public GetCurrentUserQueryHandler(IRequestContext requestContext)
        {
            _requestContext = requestContext;
        }

        public Task<SuccessDetails<CurrentUserDto>> Handle(
            GetCurrentUserQueryRequest request,
            CancellationToken cancellationToken)
        {
            if (_requestContext.UserId is null)
                throw new UnauthorizedException("Oturum bilgisi bulunamadı.");

            var claims = _requestContext.User?.Claims?.ToList()
                         ?? new List<Claim>();

            var roles = claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var permissions = claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var dto = new CurrentUserDto
            {
                Id = _requestContext.UserId,
                UserName = _requestContext.Username ?? string.Empty,
                Email = _requestContext.Email ?? string.Empty,
                Roles = roles,
                Permissions = permissions,
                IsAuthenticated = _requestContext.User?.Identity?.IsAuthenticated ?? false,
            };

            return Task.FromResult(
                ResultResponse.Success(dto, Response.Common.OperationSuccess));
        }
    }
}