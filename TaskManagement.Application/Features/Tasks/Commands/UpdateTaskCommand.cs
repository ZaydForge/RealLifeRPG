using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Features.Tasks.Commands
{
    public class UpdateTaskCommand(int id, UpdateTaskDto taskDto): IRequest<string>
    {
        public int Id { get; } = id;
       public UpdateTaskDto Request { get; } = taskDto;
    }

    public class UpdateTaskCommandHandler(
        ITaskRepository taskRepo,
        IMapper mapper,
        IDistributedCache cache) : IRequestHandler<UpdateTaskCommand, string>
    {
        public async Task<string> Handle(UpdateTaskCommand command, CancellationToken token)
        {
            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task == null)
            {
                throw new Exception("Task not found");
            }

            mapper.Map(command.Request, task);
            taskRepo.Update(task);
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", token);
            await cache.RemoveAsync($"task_{task.Id}", token);

            return "Task updated successfully";
        }
    }
}
