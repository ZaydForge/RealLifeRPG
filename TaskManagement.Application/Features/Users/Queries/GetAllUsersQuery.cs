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

namespace TaskManagement.Application.Features.Users.Queries
{
    public class GetAllUsersQuery : IRequest<IEnumerable<UserProfileDto>>
    {

    }

    public class GetAllUsersQueryHandler(
        IUserProfileRepository userRepo,
        IMapper mapper,
        IDistributedCache cache) 
        : IRequestHandler<GetAllUsersQuery, IEnumerable<UserProfileDto>>
    {
        public readonly string _cacheKey = "users_list";
        public async Task<IEnumerable<UserProfileDto>> Handle(GetAllUsersQuery query, CancellationToken token)
        {
            var cachedUsers = await cache.GetStringAsync(_cacheKey);
            if (!string.IsNullOrEmpty(cachedUsers))
            {
                var deserialized = JsonSerializer.Deserialize<IEnumerable<UserProfileDto>>(cachedUsers, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                if (deserialized is null || !deserialized.Any())
                {
                    throw new NotFoundException("Users not found");
                }

                return deserialized;
            }
            var users = mapper.Map<IEnumerable<UserProfileDto>>(await userRepo.GetAllUsersAsync());

            var serialized = JsonSerializer.Serialize(users);
            var options = new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(1),
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            };

            await cache.SetStringAsync(_cacheKey, serialized, options);

            return users;
        }
    }
}
