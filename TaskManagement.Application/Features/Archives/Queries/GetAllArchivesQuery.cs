using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Entities;
using System.Threading.Tasks;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Features.Archives.Queries
{
    public class GetAllArchivesQuery : IRequest<IEnumerable<ArchiveDto>>
    {
    }

    public class GetAllArchivesQueryHandler(
        IArchiveRepository archiveRepo,
        ITaskRepository taskRepo,
        IMapper mapper,
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo)
        : IRequestHandler<GetAllArchivesQuery, IEnumerable<ArchiveDto>>
    {
        public async Task<IEnumerable<ArchiveDto>> Handle(GetAllArchivesQuery request,
            CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var userCacheKey = $"archives_list_user_{profileId}";
            var cachedTasks = await cache.GetStringAsync(userCacheKey);

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

            var tasks = (await taskRepo.GetAllAsync()).Where(t => t.UserId == profileId).ToList();
            var expiredTasks = tasks
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
                        UserId = profileId
                    };
                    await archiveRepo.AddAsync(archive);
                    task.Status = Domain.Enums.TaskStatus.Expired;
                }
                await archiveRepo.SaveChangesAsync();
                await taskRepo.SaveChangesAsync();

                tasks.RemoveAll(t => expiredTasks.Contains(t));

                await cache.RemoveAsync($"tasks_list_user_{profileId}", cancellationToken);
                await cache.RemoveAsync(userCacheKey, cancellationToken);
            }

            var archives = mapper.Map<IEnumerable<ArchiveDto>>((await archiveRepo.GetAllAsync()).Where(a => a.UserId == profileId));
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

            await cache.SetStringAsync(userCacheKey, serialized, options);

            return archives;
        }
    }
}
