using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;
using TaskManagement.Domain.Enums;

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
            var request = createTaskCommand.Request;
            var task = mapper.Map<TaskItem>(request);
            task.ExpiresAt = request.ExpirationType switch
            {
                ExpirationType.Urgent => DateTime.UtcNow.AddDays(1),
                ExpirationType.Pending => DateTime.UtcNow.AddDays(2),
                ExpirationType.Destined => request.CustomExpirationDate,
                _ => DateTime.UtcNow.AddDays(1)
            };

            await taskRepo.AddAsync(task);
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync("tasks_list", token);

            return "Task created successfully!";
        }
    }
}
