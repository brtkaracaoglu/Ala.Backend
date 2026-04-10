using Ala.Backend.Application.Abstractions.Infrastructure.Services.Maintenance;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.Maintenance
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Maintenance")]
    [Authorize(Roles = "Admin")]
    public class MaintenanceController : ControllerBase
    {
        private readonly IAuthDataCleanupService _authDataCleanupService;

        public MaintenanceController(IAuthDataCleanupService authDataCleanupService)
        {
            _authDataCleanupService = authDataCleanupService;
        }

        [HttpPost("auth-cleanup")]
        public async Task<IActionResult> RunAuthCleanup(CancellationToken cancellationToken)
        {
            var result = await _authDataCleanupService.CleanupAsync(cancellationToken);
            return Ok(result);
        }
    }
}