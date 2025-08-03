using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Features.Tasks.Queries
{
    public class GetAllTasksQuery : IRequest<IEnumerable<TaskDto>>
    {
    }

    public class GetAllTasksQueryHandler(
        ITaskRepository taskRepo,
        IArchiveRepository archiveRepo,
        IMapper mapper,
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo)
        : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskDto>>
    {
        public async Task<IEnumerable<TaskDto>> Handle(
            GetAllTasksQuery query, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);

            var userCacheKey = $"tasks_list_user_{userProfile.Id}";
            var cachedTasks = await cache.GetStringAsync(userCacheKey);

            if (!string.IsNullOrEmpty(cachedTasks))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<TaskDto>>(cachedTasks, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("There are no tasks yet");
                }

                if(!deserialized.Any(t => t.ExpiresAt.Date < DateTime.UtcNow))
                {
                    return deserialized;

                }

            }

            var oldTasks = (await taskRepo.GetAllAsync()).Where(t => t.UserId == userProfile.Id).ToList();

            var expiredTasks = oldTasks
                    .Where(r => r.ExpiresAt.Date < DateTime.UtcNow && r.Status == Domain.Enums.TaskStatus.Active)
                    .ToList();

            if (expiredTasks.Any())
            {
                foreach (var task in expiredTasks)
                {
                    var archive = new Archive
                    {
                        Title = task.Title,
                        Description = task.Description,
                        CreatedDate = task.CreatedDate,
                        EXPValue = task.EXPValue,
                        Category = task.Category,
                        UserId = userProfile.Id,
                    };
                    await archiveRepo.AddAsync(archive);
                    task.Status = Domain.Enums.TaskStatus.Expired;
                }
                await archiveRepo.SaveChangesAsync();
                await taskRepo.SaveChangesAsync();

                await cache.RemoveAsync($"archives_list_user_{userId}", cancellationToken);
                await cache.RemoveAsync(userCacheKey, cancellationToken);
            }

            var taskDtos = mapper.Map<IEnumerable<TaskDto>>((await taskRepo.GetAllAsync()).Where(t => t.UserId == userProfile.Id));

            var serialized = JsonSerializer.Serialize(taskDtos);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(userCacheKey, serialized, options);

            return taskDtos;
        }
    }
}
