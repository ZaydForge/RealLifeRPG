using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Features.Tasks.Commands;
using TaskManagement.Application.Features.Tasks.Queries;
using TaskManagement.Application.Validations;
using TaskManagement.Dtos;
using TaskManagement.Rules;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(IMediator mediator) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tasks = await mediator.Send(new GetAllTasksQuery());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var task = await mediator.Send(new GetTaskByIdQuery(id));
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto taskDto)
        {
            var validator = new CreateTaskRequestValidator();
            var result = await validator.ValidateAsync(taskDto);
            if(!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            await mediator.Send(new CreateTaskCommand(taskDto));
            return Created();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTaskDto taskDto)
        {
            var validator = new UpdateTaskRequestValidator();
            var result = await validator.ValidateAsync(taskDto);
            if (!result.IsValid)
            {
                return BadRequest(result.Errors);
            }

            await mediator.Send(new UpdateTaskCommand(id, taskDto));
            return NoContent();
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Complete([FromRoute] int id)
        {
            var result = await mediator.Send(new CompleteTaskCommand(id));
            return Ok(result);
        }

        [HttpPost("save/{id}")]
        public async Task<IActionResult> Save([FromRoute] int id)
        {
            var result = await mediator.Send(new SaveTaskCommand(id));
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            await mediator.Send(new DeleteTaskCommand(id));
            return NoContent();
        }
    }
}
