using Ala.Backend.Application.Features.Commands.Auth.ChangePassword;
using Ala.Backend.Application.Features.Commands.Auth.ConfirmEmail;
using Ala.Backend.Application.Features.Commands.Auth.ForgotPassword;
using Ala.Backend.Application.Features.Commands.Auth.Login;
using Ala.Backend.Application.Features.Commands.Auth.Logout;
using Ala.Backend.Application.Features.Commands.Auth.RefreshToken;
using Ala.Backend.Application.Features.Commands.Auth.Register;
using Ala.Backend.Application.Features.Commands.Auth.ResendConfirmationEmail;
using Ala.Backend.Application.Features.Commands.Auth.ResetPassword;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ala.Backend.WebAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Authentication")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var allClaims = User.Claims.ToList();

            var roles = allClaims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var permissions = allClaims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .Distinct()
                .ToList();

            var userDto = new
            {
                Id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                UserName = User.Identity?.Name,
                Email = User.FindFirst(ClaimTypes.Email)?.Value,
                Roles = roles,
                Permissions = permissions
            };

            return Ok(userDto);
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
        public async Task<IActionResult> Login(LoginCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized("Refresh token bulunamadı.");

            var result = await _mediator.Send(new RefreshTokenCommandRequest
            {
                RefreshToken = refreshToken
            });

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await _mediator.Send(new LogoutCommandRequest());
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
        public async Task<IActionResult> ChangePassword(ChangePasswordCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }
    }
}