using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Dtos;
using TaskManagement.Application.Features.Users.Commands;
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

        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile([FromRoute] int id, [FromBody] UpdateUserProfileDto updateUserProfileDto)
        {
            var result = await mediator.Send(new UpdateUserProfileCommand(id, updateUserProfileDto));
                return Ok(result);
        }
    }
}
