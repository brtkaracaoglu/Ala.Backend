using Ala.Backend.Application.Features.Commands.UserRoles.AddRoleToUser;
using Ala.Backend.Application.Features.Commands.UserRoles.RemoveRoleFromUser;
using Ala.Backend.Application.Features.Queries.UserRoles.GetAllUserRoles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.UserRole
{
    [Route("api/[controller]")]
    [ApiController]
    [Tags("UserRoles")] 
    public class UserRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserRolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetAllUserRolesQueryRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRoleToUser([FromBody] AddRoleToUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpDelete("remove-role")]
        public async Task<IActionResult> RemoveRoleFromUser([FromBody] RemoveRoleFromUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}