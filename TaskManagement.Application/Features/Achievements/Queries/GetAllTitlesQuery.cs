using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Dtos;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;

namespace TaskManagement.Application.Features.Achievements.Queries
{
    public class GetAllTitlesQuery : IRequest<IEnumerable<TitleDto>>
    {
    }

    public class GetAllTitlesQueryHandler(
        IAchievementRepository achievementRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllTitlesQuery, IEnumerable<TitleDto>>
    {
        public readonly string _cacheKey = "titles_list";
        public async Task<IEnumerable<TitleDto>> Handle(GetAllTitlesQuery query,
            CancellationToken cancellationToken)
        {
            var cachedTitles = await cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedTitles))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<TitleDto>>(cachedTitles, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("Titles not found");
                }

                return deserialized;
            }

            var titles = mapper
                .Map<IEnumerable<TitleDto>>(await achievementRepo.GetTitlesAsync());

            var serialized = JsonSerializer.Serialize(titles);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(_cacheKey, serialized, options);


            return titles;
        }
    }
}
