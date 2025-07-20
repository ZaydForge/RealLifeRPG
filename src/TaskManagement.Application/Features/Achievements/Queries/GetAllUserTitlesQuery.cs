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
    public class GetAllUserTitlesQuery : IRequest<IEnumerable<UserTitleDto>>
    {
    }

    public class GetAllUserTitlesQueryHandler(
        IAchievementRepository achievementRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllUserTitlesQuery, IEnumerable<UserTitleDto>>
    {
        public readonly string _cacheKey = "userTitles_list";
        public async Task<IEnumerable<UserTitleDto>> Handle(GetAllUserTitlesQuery query,
            CancellationToken cancellationToken)
        {
            var cachedUserTitles = await cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedUserTitles))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<UserTitleDto>>(cachedUserTitles, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("User Titles not found");
                }

                return deserialized;
            }

            var userTitles = mapper
                .Map<IEnumerable<UserTitleDto>>(await achievementRepo.GetUserTitlesAsync());

            var serialized = JsonSerializer.Serialize(userTitles);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(_cacheKey, serialized, options);


            return userTitles;
        }
    }
}
