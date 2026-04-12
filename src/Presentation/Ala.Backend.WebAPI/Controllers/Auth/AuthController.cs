using Ala.Backend.Application.Features.Commands.Auth.ChangePassword;
using Ala.Backend.Application.Features.Commands.Auth.ConfirmEmail;
using Ala.Backend.Application.Features.Commands.Auth.ForgotPassword;
using Ala.Backend.Application.Features.Commands.Auth.Login;
using Ala.Backend.Application.Features.Commands.Auth.Logout;
using Ala.Backend.Application.Features.Commands.Auth.LogoutAll;
using Ala.Backend.Application.Features.Commands.Auth.RefreshToken;
using Ala.Backend.Application.Features.Commands.Auth.Register;
using Ala.Backend.Application.Features.Commands.Auth.ResendConfirmationEmail;
using Ala.Backend.Application.Features.Commands.Auth.ResetPassword;
using Ala.Backend.Application.Features.Queries.Auth.GetCurrentUser;
using Ala.Backend.Presentation.Abstractions;
using Ala.Backend.WebAPI.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Authentication")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ITokenCookieService _tokenCookieService;

        public AuthController(
            IMediator mediator,
            ITokenCookieService tokenCookieService)
        {
            _mediator = mediator;
            _tokenCookieService = tokenCookieService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentUserQueryRequest(), cancellationToken);
            return Ok(result);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ServiceFilter(typeof(TokenCookieFilter))]
        public async Task<IActionResult> Login(LoginCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [ServiceFilter(typeof(TokenCookieFilter))]
        public async Task<IActionResult> Refresh()
        {
            var request = new RefreshTokenCommandRequest
            {
                RefreshToken = _tokenCookieService.GetRefreshToken() ?? string.Empty
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        [ServiceFilter(typeof(TokenCookieFilter))]
        public async Task<IActionResult> Logout()
        {
            var request = new LogoutCommandRequest
            {
                RefreshToken = _tokenCookieService.GetRefreshToken()
            };

            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("logout-all")]
        [Authorize]
        [ServiceFilter(typeof(TokenCookieFilter))]
        public async Task<IActionResult> LogoutAll(LogoutAllCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ServiceFilter(typeof(TokenCookieFilter))]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("resend-confirmation-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmationEmail(ResendConfirmationEmailCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ServiceFilter(typeof(TokenCookieFilter))]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}