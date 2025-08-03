using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Application.Services;

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
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo) : IRequestHandler<UpdateTaskCommand, string>
    {
        public async Task<string> Handle(UpdateTaskCommand command, CancellationToken token)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task == null || task.UserId != profileId)
            {
                throw new Exception("Task not found");
            }

            mapper.Map(command.Request, task);
            taskRepo.Update(task);
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync($"tasks_list_user_{profileId}", token);
            await cache.RemoveAsync($"task_{task.Id}_user_{profileId}", token);

            return "Task updated successfully";
        }
    }
}
