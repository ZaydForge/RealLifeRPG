using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Features.TaskLogs.Queries
{
    public class GetAllTaskLogsQuery : IRequest<IEnumerable<TaskLogDto>>
    {
    }

    public class GetAllTaskLogsQueryHandler(
        ITaskLogRepository taskLogRepo,
        IMapper mapper,
        IDistributedCache cache) :
        IRequestHandler<GetAllTaskLogsQuery, IEnumerable<TaskLogDto>>
    {
        public readonly string _cacheKey = "task_logs_list";
        public async Task<IEnumerable<TaskLogDto>> Handle(GetAllTaskLogsQuery query, CancellationToken cancellationToken)
        {
            var cachedTaskLogs = await cache.GetStringAsync(_cacheKey);
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
            var taskLogs = mapper.Map<IEnumerable<TaskLogDto>>(await taskLogRepo.GetTaskLogsAsync());
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

            await cache.SetStringAsync(_cacheKey, serialized, options);

            return taskLogs;
        }
    }
}
