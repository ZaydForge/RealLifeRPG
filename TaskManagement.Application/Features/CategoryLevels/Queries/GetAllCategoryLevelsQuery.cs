using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Exceptions;
using TaskManagement.Domain.Interfaces;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Features.CategoryLevels.Queries
{
    public class GetAllCategoryLevelsQuery : IRequest<IEnumerable<CategoryLevelDto>>
    {
    }

    public class GetAllCategoryLevelsQueryHandler(
        ICategoryLevelRepository categoryRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllCategoryLevelsQuery, IEnumerable<CategoryLevelDto>>
    {
        public readonly string _cacheKey = "categories_list";
        public async Task<IEnumerable<CategoryLevelDto>> Handle(GetAllCategoryLevelsQuery query,
            CancellationToken cancellationToken)
        {
            var cachedCategories = await cache.GetStringAsync(_cacheKey);
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
                .Map<IEnumerable<CategoryLevelDto>>(await categoryRepo.GetAllAsync());

            var serialized = JsonSerializer.Serialize(categoryLevels);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(_cacheKey, serialized, options);


            return categoryLevels;

        }
    }
}
