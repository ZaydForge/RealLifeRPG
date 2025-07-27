using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Dtos;
using TaskManagement.Application.Features.Users.Commands;
using TaskManagement.Application.Features.Users.Queries;
using TaskManagement.Application.Services;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController(IMediator mediator, IFileStorageService storageService) : ControllerBase
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
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile([FromRoute] int id, [FromForm] UpdateUserProfileDto updateUserProfileDto)
        {
            var result = await mediator.Send(new UpdateUserProfileCommand(id, updateUserProfileDto));
            return Ok(result);
        }

        [HttpGet("photo/{objectName}")]
        public async Task<IActionResult> GetPhoto([FromRoute] string objectName)
        {
            string bucket = "profile-photo";

            var stream = await storageService.DownloadFileAsync(bucket, objectName);
            if (stream == null)
                return NotFound();

            return File(stream, "application/octet-stream", objectName);
        }
    }
}
