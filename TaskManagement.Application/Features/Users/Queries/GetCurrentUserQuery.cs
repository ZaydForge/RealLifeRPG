using AutoMapper;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManagement.Application.Exceptions;
using TaskManagement.Persistence.RepositoryInterfaces;
using TaskManagement.Dtos;
using TaskManagement.Application.Services;

namespace TaskManagement.Application.Features.Users.Queries
{
    public class GetCurrentUserQuery : IRequest<UserProfileDto>
    {

    }

    public class GetCurrentUserQueryHandler(
       IUserProfileRepository userRepo,
       IMapper mapper,
       IDistributedCache cache,
       ICurrentUserService currentUserService)
       : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
    {
        public async Task<UserProfileDto> Handle(GetCurrentUserQuery query, CancellationToken token)
        {
            var currentUserId = currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            var userProfile = await userRepo.GetByUserIdAsync(currentUserId.Value);
            var profileId = userProfile.Id;

            var cacheKey = $"current_user_{profileId}";
            var cachedUser = await cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedUser))
            {
                var deserialized = JsonSerializer.Deserialize<UserProfileDto>(cachedUser, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null)
                {
                    throw new NotFoundException("User not found");
                }

                return deserialized;
            }
            var user = mapper.Map<UserProfileDto>(userProfile);

            var serialized = JsonSerializer.Serialize(user);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(cacheKey, serialized, options);

            return user;
        }
    }
}
