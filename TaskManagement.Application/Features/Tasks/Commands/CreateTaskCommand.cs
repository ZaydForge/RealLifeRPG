using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Application.Features.Tasks.Commands
{
    public class CreateTaskCommand(CreateTaskDto createTaskDto) : IRequest<string>
    {
        public CreateTaskDto Request { get; } = createTaskDto;
    }

    public class CreateTaskCommandHanlder(
        ITaskRepository taskRepo,
        IMapper mapper,
        IDistributedCache cache) : IRequestHandler<CreateTaskCommand, string>
    {
        public async Task<string> Handle(CreateTaskCommand createTaskCommand, CancellationToken token)
        {

            var task = mapper.Map<TaskItem>(createTaskCommand.Request);

            await taskRepo.AddAsync(task);
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", token);

            return "Task created successfully!";
        }
    }
}
