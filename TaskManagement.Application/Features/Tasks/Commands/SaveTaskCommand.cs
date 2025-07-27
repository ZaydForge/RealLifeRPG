using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Application.Features.Tasks.Commands
{
    public class SaveTaskCommand : IRequest<string>
    {
        public int Id { get; set; }
        public SaveTaskCommand(int id)
        {
            Id = id;
        }
    }

    public class SaveTaskCommandHandler(ITaskRepository taskRepo, IDistributedCache cache)
        : IRequestHandler<SaveTaskCommand, string>
    {
        public async Task<string> Handle(SaveTaskCommand command, CancellationToken cancellationToken)
        {
            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task == null)
            {
                throw new NotFoundException("Task not found");
            }
            task.IsSaved = true;
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", cancellationToken);
            await cache.RemoveAsync($"task_{task.Id}", cancellationToken);

            return "Task is saved!";

        }
    }
}
