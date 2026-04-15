using Ala.Backend.Application.Features.Queries.Permissions.GetPagedPermissions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.Permission
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Permissions")]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged([FromQuery] GetPagedPermissionsQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}
