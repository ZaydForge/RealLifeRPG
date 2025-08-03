using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Application.Services;

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

    public class DeleteTaskCommandHandler(
        ITaskRepository taskRepo,
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo)
        : IRequestHandler<DeleteTaskCommand, string>
    {
        public async Task<string> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var task = await taskRepo.GetByIdAsync(command.Id);
            if (task == null || task.UserId != profileId)
            {
                throw new NotFoundException("Task not found");
            }
            await taskRepo.Delete(task);
            await taskRepo.SaveChangesAsync();

            await cache.RemoveAsync($"tasks_list_user_{profileId}", cancellationToken);
            await cache.RemoveAsync($"task_{task.Id}_user_{profileId}", cancellationToken);

            return "Task deleted succesfully";

        }
    }
}
