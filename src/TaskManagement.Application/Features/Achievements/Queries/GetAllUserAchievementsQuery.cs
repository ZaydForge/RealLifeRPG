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
    public class GetAllUserAchievementsQuery : IRequest<IEnumerable<UserAchievementDto>>
    {
    }

    public class GetAllUserAchievementsQueryHandler(
        IAchievementRepository achievementRepo,
        IMapper mapper,
        IDistributedCache cache)
        : IRequestHandler<GetAllUserAchievementsQuery, IEnumerable<UserAchievementDto>>
    {
        public readonly string _cacheKey = "userAchievements_list";
        public async Task<IEnumerable<UserAchievementDto>> Handle(GetAllUserAchievementsQuery query,
            CancellationToken cancellationToken)
        {
            var cachedUserAchievements = await cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedUserAchievements))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<UserAchievementDto>>(cachedUserAchievements, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("User Achievements not found");
                }

                return deserialized;
            }

            var userAchievements = mapper
                .Map<IEnumerable<UserAchievementDto>>(await achievementRepo.GetUserAchievementsAsync());

            var serialized = JsonSerializer.Serialize(userAchievements);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(_cacheKey, serialized, options);


            return userAchievements;
        }
    }
}
