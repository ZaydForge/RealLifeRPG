using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class TasksController(IMediator mediator) : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tasks = await mediator.Send(new GetAllTasksQuery());
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch tasks", error = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var task = await mediator.Send(new GetTaskByIdQuery(id));
                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }
                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch task", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto taskDto)
        {
            try
            {
                if (taskDto == null)
                {
                    return BadRequest(new { message = "Task data is required" });
                }

                var validator = new CreateTaskRequestValidator();
                var result = await validator.ValidateAsync(taskDto);
                if (!result.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

                var createdTask = await mediator.Send(new CreateTaskCommand(taskDto));
                return Created();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create task", error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateTaskDto taskDto)
        {
            try
            {
                if (taskDto == null)
                {
                    return BadRequest(new { message = "Task data is required" });
                }

                var validator = new UpdateTaskRequestValidator();
                var result = await validator.ValidateAsync(taskDto);
                if (!result.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Validation failed",
                        errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                    });
                }

                await mediator.Send(new UpdateTaskCommand(id, taskDto));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update task", error = ex.Message });
            }
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Complete([FromRoute] int id)
        {
            try
            {
                var result = await mediator.Send(new CompleteTaskCommand(id));
                return Ok(new { message = "Task completed successfully", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to complete task", error = ex.Message });
            }
        }

        [HttpPost("save/{id}")]
        public async Task<IActionResult> Save([FromRoute] int id)
        {
            try
            {
                var result = await mediator.Send(new SaveTaskCommand(id));
                return Ok(new { message = "Task saved successfully", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to save task", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                await mediator.Send(new DeleteTaskCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete task", error = ex.Message });
            }
        }
    }
}
