using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;

namespace TaskManagement.Application.Features.Archives.Queries
{
    public class GetAllArchivesQuery : IRequest<IEnumerable<ArchiveDto>>
    {
    }

    public class GetAllArchivesQueryHandler(
        IArchiveRepository archiveRepo,
        ITaskRepository taskRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllArchivesQuery, IEnumerable<ArchiveDto>>
    {
        public readonly string _cacheKey = "archives_list";
        public async Task<IEnumerable<ArchiveDto>> Handle(GetAllArchivesQuery request,
            CancellationToken cancellationToken)
        {
            var cachedTasks = await cache.GetStringAsync(_cacheKey);

            if (!string.IsNullOrEmpty(cachedTasks))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<ArchiveDto>>(cachedTasks, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("Archive is empty");
                }

                return deserialized;
            }

            var tasks = (await taskRepo.GetAllAsync()).ToList();

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
                        Category = task.Category,
                    };
                    await archiveRepo.AddAsync(archive);
                    task.Status = Domain.Enums.TaskStatus.Expired;
                }
                await archiveRepo.SaveChangesAsync();
                await taskRepo.SaveChangesAsync();

                tasks.RemoveAll(t => expiredTasks.Contains(t));

                await cache.RemoveAsync("tasks_list", cancellationToken);
                await cache.RemoveAsync(_cacheKey, cancellationToken);
            }

            var archives = mapper.Map<IEnumerable<ArchiveDto>>(await archiveRepo.GetAllAsync());
            if (!archives.Any() || archives == null)
            {
                throw new NotFoundException("Archive is empty.");
            }

            var serialized = JsonSerializer.Serialize(archives);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(_cacheKey, serialized, options);

            return archives;
        }
    }
}
