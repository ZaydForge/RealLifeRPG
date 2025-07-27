using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Application.Features.Tasks.Queries
{
    public class GetAllTasksQuery : IRequest<IEnumerable<TaskDto>>
    {
    }

    public class GetAllTasksQueryHandler(
        ITaskRepository taskRepo,
        IArchiveRepository archiveRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllTasksQuery, IEnumerable<TaskDto>>
    {
        public readonly string _cacheKey = "tasks_list";
        public async Task<IEnumerable<TaskDto>> Handle(
            GetAllTasksQuery query, CancellationToken cancellationToken)
        {
            var cachedTasks = await cache.GetStringAsync(_cacheKey);

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

                return deserialized;
            }

            var tasks = (await taskRepo.GetAllActiveAsync()).ToList();

            var expiredTasks = tasks
                    .Where(r => r.CreatedDate.Date > r.ExpiresAt.Date)
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
                        Category = task.Category
                    };
                    await archiveRepo.AddAsync(archive);
                    task.Status = Domain.Enums.TaskStatus.Expired;
                }
                await archiveRepo.SaveChangesAsync();
                await taskRepo.SaveChangesAsync();

                tasks.RemoveAll(t => expiredTasks.Contains(t));
                await cache.RemoveAsync("archives_list", cancellationToken);
                await cache.RemoveAsync(_cacheKey, cancellationToken);
            }

            var taskDtos = mapper.Map<IEnumerable<TaskDto>>(tasks);

            var serialized = JsonSerializer.Serialize(taskDtos);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(_cacheKey, serialized, options);

            return taskDtos;
        }
    }
}
