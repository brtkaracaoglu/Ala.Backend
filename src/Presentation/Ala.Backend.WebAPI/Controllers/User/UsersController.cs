using Ala.Backend.Application.Features.Commands.Users.CreateUser;
using Ala.Backend.Application.Features.Commands.Users.DeleteUser;
using Ala.Backend.Application.Features.Commands.Users.LockUser;
using Ala.Backend.Application.Features.Commands.Users.SetActiveUser;
using Ala.Backend.Application.Features.Commands.Users.UnlockUser;
using Ala.Backend.Application.Features.Commands.Users.UpdateUser;
using Ala.Backend.Application.Features.Queries.Users.GetAllUsers;
using Ala.Backend.Application.Features.Queries.Users.GetUserById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ala.Backend.WebAPI.Controllers.User
{
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommandRequest request)
        {
            var response = await _mediator.Send(request);       
            return StatusCode(response.Status, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetAllUsersQueryRequest());
            return StatusCode(response.Status, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var request = new GetUserByIdQueryRequest { Id = id };
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserCommandRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteUserCommandRequest { Id = id };
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }


        [HttpPost("{id}/lock")]
        public async Task<IActionResult> LockUser([FromRoute] int id, [FromBody] LockUserCommandRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }

        [HttpPost("{id}/unlock")]
        public async Task<IActionResult> UnlockUser([FromRoute] int id)
        {
            var request = new UnlockUserCommandRequest { Id = id };
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }

        [HttpPut("{id}/active-status")]
        public async Task<IActionResult> SetActiveStatus([FromRoute] int id, [FromBody] SetActiveUserCommandRequest request)
        {
            request.Id = id;
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }
    }
}