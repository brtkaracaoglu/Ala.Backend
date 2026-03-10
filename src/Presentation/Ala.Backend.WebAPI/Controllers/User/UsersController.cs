using Ala.Backend.Application.Features.Commands.Users.CreateUser;
using Ala.Backend.Application.Features.Commands.Users.DeleteUser;
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

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommandRequest request)
        {
            var response = await _mediator.Send(request);

            // ResultResponse.Created metodu Status = 201 döndüğü için, 
            // StatusCode metodu HTTP 201 Created yanıtı üretecektir.
            return StatusCode(response.Status, response);
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetAllUsersQueryRequest());

            // HTTP 200 OK
            return StatusCode(response.Status, response);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var request = new GetUserByIdQueryRequest { Id = id };
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateUserCommandRequest request)
        {
            // Güvenlik: URL'den gelen ID'yi, Request Body'den gelen ID'nin üzerine yazıyoruz
            // Böylece istemci URL'de başka, Body'de başka ID gönderemez.
            request.Id = id;
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var request = new DeleteUserCommandRequest { Id = id };
            var response = await _mediator.Send(request);

            return StatusCode(response.Status, response);
        }
    }
}