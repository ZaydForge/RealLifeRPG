using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Features.CategoryLevels.Queries
{
    public class GetAllUserCategoriesQuery : IRequest<IEnumerable<CategoryLevelDto>>
    {
    }

    public class GetAllUserCategoriesQueryHandler(
        ICategoryLevelRepository categoryRepo,
        IMapper mapper,
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo)
        : IRequestHandler<GetAllUserCategoriesQuery, IEnumerable<CategoryLevelDto>>
    {
        public async Task<IEnumerable<CategoryLevelDto>> Handle(GetAllUserCategoriesQuery query,
            CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var userCacheKey = $"categories_list_user_{profileId}";
            var cachedCategories = await cache.GetStringAsync(userCacheKey);
            if (!string.IsNullOrEmpty(cachedCategories))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<CategoryLevelDto>>(cachedCategories, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("Categories not found");
                }

                return deserialized;
            }

            var categoryLevels = mapper
                .Map<IEnumerable<CategoryLevelDto>>((await categoryRepo.GetAllAsync()).Where(c => c.UserId == profileId));

            var serialized = JsonSerializer.Serialize(categoryLevels);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(userCacheKey, serialized, options);


            return categoryLevels;

        }
    }
}
