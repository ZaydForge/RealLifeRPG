using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Application.Features.Tasks.Queries
{
    public class GetTaskByIdQuery(int id) : IRequest<TaskDto>
    {
        public int Id { get; } = id;
    }

    public class GetTaskByIdQueryHandler(
        ITaskRepository taskRepo,
        IArchiveRepository archiveRepo,
        IMapper mapper,
        IDistributedCache cache) : IRequestHandler<GetTaskByIdQuery, TaskDto>
    {
        public async Task<TaskDto> Handle(GetTaskByIdQuery query, CancellationToken token)
        {
            var cacheKey = $"task_{query.Id}";
            var cached = await cache.GetStringAsync(cacheKey, token);
            if (!string.IsNullOrEmpty(cached))
            {
                var deserialized = JsonSerializer.Deserialize<TaskDto>(cached, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null)
                {
                    throw new NotFoundException("Task not found");
                }

                if (deserialized.CreatedDate.Date == DateTime.Today)
                {
                    return deserialized;
                }
            }

            var task = await taskRepo.GetByIdAsync(query.Id);
            if (task.CreatedDate.Day < DateTime.UtcNow.Day)
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
                await taskRepo.Delete(task);

                await archiveRepo.SaveChangesAsync();
                await taskRepo.SaveChangesAsync();

                await cache.RemoveAsync("archives_list", token);
                throw new Exception("Task has expired and has been moved to archive.");
            }
            if (task == null)
            {
                throw new NotFoundException("Task not found");
            } 

            var taskDto = mapper.Map<TaskDto>(task);
            var serialized = JsonSerializer.Serialize(taskDto);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(cacheKey, serialized, options);

            return taskDto;
        }
    }

}
