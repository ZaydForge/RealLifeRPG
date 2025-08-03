using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.CategoryLevels.Queries;
using TaskManagement.Entities;

namespace TaskManagement.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryLevelsController(IMediator mediator) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tasks = await mediator.Send(new GetAllUserCategoriesQuery());
        return Ok(tasks);
    }

}
