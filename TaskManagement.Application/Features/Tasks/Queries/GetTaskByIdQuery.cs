﻿using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskManagement.Application.Exceptions;
using TaskManagement.Dtos;
using TaskManagement.Entities;
using TaskManagement.Persistence.RepositoryInterfaces;

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

                return deserialized;
            }

            var task = await taskRepo.GetByIdAsync(query.Id);
            if (task == null)
            {
                throw new NotFoundException("Task not found");
            } 
            if (task.CreatedDate.Date > DateTime.UtcNow.Date)
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

                await archiveRepo.SaveChangesAsync();
                await taskRepo.SaveChangesAsync();

                await cache.RemoveAsync("archives_list", token);
                await cache.RemoveAsync("tasks_list", token);
                await cache.RemoveAsync(cacheKey, token);
                throw new Exception("Task has expired and has been moved to archive.");
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
