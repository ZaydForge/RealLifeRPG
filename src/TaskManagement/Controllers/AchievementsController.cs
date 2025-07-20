using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Achievements.Queries;

namespace TaskManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController(IMediator mediator) : ControllerBase
    {
        [HttpGet("Achievements")]
        public async Task<IActionResult> GetAchievements()
        {
            var achievements = await mediator.Send(new GetAllAchievementsQuery());

            return Ok(achievements);
        }

        [HttpGet("UserAchievements")]
        public async Task<IActionResult> GetUserAchievements()
        {
            var userAchievements = await mediator.Send(new GetAllUserAchievementsQuery());

            return Ok(userAchievements);
        }

        [HttpGet("Titles")]
        public async Task<IActionResult> GetTitles()
        {
            var titles = await mediator.Send(new GetAllTitlesQuery());

            return Ok(titles);
        }

        [HttpGet("UserTitles")]
        public async Task<IActionResult> GetUserTitles()
        {
            var userTitle = await mediator.Send(new GetAllUserTitlesQuery());

            return Ok(userTitle);
        }
    }
}
