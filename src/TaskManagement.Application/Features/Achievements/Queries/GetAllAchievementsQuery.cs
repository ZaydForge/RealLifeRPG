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
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Features.Achievements.Queries
{
    public class GetAllAchievementsQuery : IRequest<IEnumerable<AchievementDto>>
    {
    }

    public class GetAllAchievementsQueryHandler(
        IAchievementRepository achievementRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllAchievementsQuery, IEnumerable<AchievementDto>>
    {
        public readonly string _cacheKey = "achievements_list";
        public async Task<IEnumerable<AchievementDto>> Handle(GetAllAchievementsQuery query,
            CancellationToken cancellationToken)
        {
            var cachedAchievements = await cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedAchievements))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<AchievementDto>>(cachedAchievements, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("Achievements not found");
                }

                return deserialized;
            }

            var achievements = mapper
                .Map<IEnumerable<AchievementDto>>(await achievementRepo.GetAchievementsAsync());

            var serialized = JsonSerializer.Serialize(achievements);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(_cacheKey, serialized, options);


            return achievements;
        }
    }
}
