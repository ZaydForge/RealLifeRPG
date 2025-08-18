using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using TaskManagement.Application.Exceptions;
using TaskManagement.Application.Services;
using TaskManagement.Persistence.RepositoryInterfaces;
namespace TaskManagement.Application.Features.Tasks.Commands
{

    namespace TaskManagement.Application.Features.Tasks.Commands
    {
        public class UnsaveTaskCommand : IRequest<string>
        {
            public int Id { get; set; }
            public UnsaveTaskCommand(int id)
            {
                Id = id;
            }
        }

        public class UnsaveTaskCommandHandler(
            ITaskRepository taskRepo,
            IDistributedCache cache,
            ICurrentUserService currentUserService,
            IUserProfileRepository profileRepo)
            : IRequestHandler<UnsaveTaskCommand, string>
        {
            public async Task<string> Handle(UnsaveTaskCommand command, CancellationToken cancellationToken)
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
                task.IsSaved = false;
                await taskRepo.SaveChangesAsync();

                await cache.RemoveAsync($"tasks_list_user_{profileId}", cancellationToken);
                await cache.RemoveAsync($"task_{task.Id}_user_{profileId}", cancellationToken);

                return "Task is unsaved!";

            }
        }
    }

}
