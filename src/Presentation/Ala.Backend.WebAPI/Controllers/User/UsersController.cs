using Ala.Backend.Application.Features.Commands.Users.CreateUser;
using Ala.Backend.Application.Features.Commands.Users.DeleteUser;
using Ala.Backend.Application.Features.Commands.Users.LockUser;
using Ala.Backend.Application.Features.Commands.Users.SetActiveUser;
using Ala.Backend.Application.Features.Commands.Users.UnlockUser;
using Ala.Backend.Application.Features.Commands.Users.UpdateUser;
using Ala.Backend.Application.Features.Queries.Users.GetUserById;
using Ala.Backend.Application.Features.Queries.Users.GetUsers;
using Ala.Backend.WebAPI.Authorization.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.User
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [Tags("Users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        //[PermissionAuthorize(Permissions.Users.List)]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQueryRequest request)
        {
            var result = await _mediator.Send(request);
            return Ok(result);
        }


        [HttpGet("{id}")]
        [PermissionAuthorize(Permissions.Users.View)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var request = new GetUserByIdQueryRequest { Id = id };
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateUserCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPut("{id}/update")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserCommandRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteUserCommandRequest { Id = id };
            var response = await _mediator.Send(request);

            return Ok(response);
        }


        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUser([FromRoute] int id, [FromBody] LockUserCommandRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUser([FromRoute] int id)
        {
            var request = new UnlockUserCommandRequest { Id = id };
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpPut("{id}/active-status")]
        public async Task<IActionResult> SetActiveStatus([FromRoute] int id, [FromBody] SetActiveUserCommandRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}