using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Users.Queries;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await mediator.Send(new GetAllUsersQuery()));
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrent()
        {
            return Ok(await mediator.Send(new GetCurrentUserQuery()));
        }
    }
}
