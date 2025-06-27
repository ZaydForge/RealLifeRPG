using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Interfaces;

namespace TaskManagement.Application.Features.Tasks.Commands
{
    public class DeleteTaskCommand : IRequest<string>
    {
        public int Id { get; set; }
        public DeleteTaskCommand(int id)
        {
            Id = id;
        }
    }

    public class DeleteTaskCommandHandler(ITaskRepository taskRepo, IDistributedCache cache)
        : IRequestHandler<DeleteTaskCommand, string>
    {
        public async Task<string> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task == null)
            {
                throw new NotFoundException("Task not found");
            }
            await taskRepo.Delete(task);
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", cancellationToken);
            await cache.RemoveAsync($"task_{task.Id}", cancellationToken);

            return "Task deleted succesfully";

        }
    }
}
