using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Application.Contracts.Auth;
using Ala.Backend.Presentation.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ala.Backend.WebAPI.Filters
{
    public sealed class TokenCookieFilter : IAsyncActionFilter
    {
        private readonly ITokenCookieService _tokenCookieService;

        public TokenCookieFilter(ITokenCookieService tokenCookieService)
        {
            _tokenCookieService = tokenCookieService;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var executedContext = await next();

            if (executedContext.Result is not ObjectResult objectResult)
                return;

            if (objectResult.Value is not ISuccessDetails successResult)
                return;

            if (successResult.DataObject is not ITokenCookieMutation cookieMutation)
                return;

            ApplyCookieMutation(cookieMutation);
        }

        private void ApplyCookieMutation(ITokenCookieMutation mutation)
        {
            if (mutation.ClearAccessTokenCookie)
                _tokenCookieService.ClearAccessToken();

            if (mutation.ClearRefreshTokenCookie)
                _tokenCookieService.ClearRefreshToken();

            if (!string.IsNullOrWhiteSpace(mutation.AccessToken) &&
                mutation.AccessTokenExpiresAtUtc.HasValue)
            {
                _tokenCookieService.SetAccessToken(
                    mutation.AccessToken,
                    mutation.AccessTokenExpiresAtUtc.Value);
            }

            if (!string.IsNullOrWhiteSpace(mutation.RefreshToken) &&
                mutation.RefreshTokenExpiresAtUtc.HasValue)
            {
                _tokenCookieService.SetRefreshToken(
                    mutation.RefreshToken,
                    mutation.RefreshTokenExpiresAtUtc.Value);
            }
        }
    }
}