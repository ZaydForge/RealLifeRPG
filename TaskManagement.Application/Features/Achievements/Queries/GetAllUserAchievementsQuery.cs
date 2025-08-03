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
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Features.Achievements.Queries
{
    public class GetAllUserAchievementsQuery : IRequest<IEnumerable<UserAchievementDto>>
    {
    }

    public class GetAllUserAchievementsQueryHandler(
        IAchievementRepository achievementRepo,
        IMapper mapper,
        IDistributedCache cache,
        ICurrentUserService currentUserService,
        IUserProfileRepository profileRepo)
        : IRequestHandler<GetAllUserAchievementsQuery, IEnumerable<UserAchievementDto>>
    {
        public async Task<IEnumerable<UserAchievementDto>> Handle(GetAllUserAchievementsQuery query,
            CancellationToken cancellationToken)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                throw new UnauthorizedAccessException("User not authenticated");

            var userProfile = await profileRepo.GetByUserIdAsync(userId);
            var profileId = userProfile.Id;

            var userCacheKey = $"userAchievements_list_user_{profileId}";
            var cachedUserAchievements = await cache.GetStringAsync(userCacheKey);
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
                .Map<IEnumerable<UserAchievementDto>>(await achievementRepo.GetUserAchievementsAsync(profileId));

            var serialized = JsonSerializer.Serialize(userAchievements);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };
            await cache.SetStringAsync(userCacheKey, serialized, options);


            return userAchievements;
        }
    }
}
