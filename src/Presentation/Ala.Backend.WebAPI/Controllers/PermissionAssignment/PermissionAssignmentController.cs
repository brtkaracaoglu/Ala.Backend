using Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToRole;
using Ala.Backend.Application.Features.Commands.PermissionAssignment.AssignPermissionToUser;
using Ala.Backend.Application.Features.Commands.PermissionAssignment.RemovePermissionFromRole;
using Ala.Backend.Application.Features.Commands.PermissionAssignment.RemovePermissionFromUser;
using Ala.Backend.Application.Features.Queries.PermissionAssignment.GetRolePermission;
using Ala.Backend.Application.Features.Queries.PermissionAssignment.GetUserPermission;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.PermissionAssignment
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Tags("Permission Assignments")]
    public class PermissionAssignmentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionAssignmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignToRole([FromBody] AssignPermissionToRoleCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveFromRole([FromBody] RemovePermissionFromRoleCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("role-permissions")]
        public async Task<IActionResult> GetRolePermissions(int roleId)
        {
            var query = new GetRolePermissionsQueryRequest { RoleId = roleId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        [HttpPost("assign-user")]
        public async Task<IActionResult> AssignToUser([FromBody] AssignPermissionToUserCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpPost("remove-user")]
        public async Task<IActionResult> RemoveFromUser([FromBody] RemovePermissionFromUserCommandRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [HttpGet("user-permissions")]
        public async Task<IActionResult> GetUserPermissions(int userId)
        {
            var query = new GetUserPermissionsQueryRequest { UserId = userId };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
