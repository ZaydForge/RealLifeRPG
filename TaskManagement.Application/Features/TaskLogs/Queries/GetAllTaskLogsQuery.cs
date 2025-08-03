using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Features.TaskLogs.Queries
{
    public class GetAllTaskLogsQuery : IRequest<IEnumerable<TaskLogDto>>
    {
    }

    public class GetAllTaskLogsQueryHandler(
        ITaskLogRepository taskLogRepo,
        IMapper mapper,
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo) :
        IRequestHandler<GetAllTaskLogsQuery, IEnumerable<TaskLogDto>>
    {
        public async Task<IEnumerable<TaskLogDto>> Handle(GetAllTaskLogsQuery query, CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var userCacheKey = $"task_logs_list_user_{profileId}";
            var cachedTaskLogs = await cache.GetStringAsync(userCacheKey);
            if (!string.IsNullOrEmpty(cachedTaskLogs))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<TaskLogDto>>(cachedTaskLogs , new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("There are no task logs yet");
                }

                return deserialized;
            }
            var taskLogs = mapper.Map<IEnumerable<TaskLogDto>>(await taskLogRepo.GetTaskLogsAsync(profileId));
            if(taskLogs == null || !taskLogs.Any())
            {
                throw new NotFoundException("There are no completed tasks yet");
            }

            var serialized = JsonSerializer.Serialize(taskLogs);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(userCacheKey, serialized, options);

            return taskLogs;
        }
    }
}
